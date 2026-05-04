## Context

目前 `OrderService.CreateAsync` 在建立訂單時只做庫存驗證（`stock < quantity` 拋例外），不做任何扣減。`Order.Status` 是純字串，沒有合法轉換規則。在高並發情況下，兩筆訂單可能同時通過庫存驗證卻都建立成功，造成超賣。管理員也缺乏更新訂單狀態的 API。

## Goals / Non-Goals

**Goals:**
- 訂單建立時以原子方式保留庫存（`ReservedStock`），防止超賣
- 定義訂單狀態機與合法轉換，由服務層強制執行
- 管理員可透過 API 推進訂單狀態，觸發正式庫存扣減或庫存歸還
- 資料庫層使用 row-level 並發控制確保庫存保留的原子性

**Non-Goals:**
- 支付閘道整合
- 自動取消逾期未確認訂單（背景排程）
- 訂單歷史 / 稽核日誌

## Decisions

### 1. 庫存保留策略：Soft Reservation（保留欄位）vs. 直接扣減

**選擇：Soft Reservation — 在 `Product` 加入 `ReservedStock` 欄位**

| 方案 | 優點 | 缺點 |
|------|------|------|
| 訂單建立時直接扣減 stock | 邏輯簡單 | 訂單若最終取消，庫存已扣需歸還；Pending 期間 stock 顯示偏低 |
| Soft Reservation（本方案） | 可用庫存顯示準確（stock - reserved）；確認時才正式扣 | 需維護兩個欄位，查詢略複雜 |
| 無庫存保留（樂觀鎖重試） | 不需額外欄位 | 高並發下用戶體驗差（訂單提交後才被拒） |

保留策略最符合代購平台情境：商品數量有限，管理員需要先確認貨源後才正式扣庫。

### 2. 並發安全：Optimistic Concurrency via EF Core RowVersion

在 `Product` 加入 `[Timestamp] byte[] RowVersion` 欄位。`UpdateReservedStockAsync` 更新後若發生並發衝突（`DbUpdateConcurrencyException`），由 service 層捕捉並以 `InsufficientStockException` 回傳，讓呼叫方重試或回報錯誤。

不使用悲觀鎖（`SELECT FOR UPDATE`）以避免長時間持鎖影響整體吞吐。

### 3. 訂單狀態：enum vs. string

**選擇：改用 `OrderStatus` enum，EF Core 以 `HasConversion<string>()` 儲存為字串**

- JSON 序列化仍輸出字串（`"Pending"`），不破壞現有 API 契約
- 編譯期防止無效狀態值
- 狀態轉換規則集中在 domain 層驗證

### 4. 狀態轉換規則

```
Pending  → Confirmed  (Admin)
Pending  → Cancelled  (Admin or Owner)
Confirmed → Shipped   (Admin)
Confirmed → Cancelled (Admin)
Shipped  → Completed  (Admin)
```

任何非法轉換拋 `InvalidOrderStatusTransitionException`。

### 5. API 設計

```
PATCH /api/orders/{id}/status
Body: { "status": "Confirmed" }
Auth: Admin（Pending→Confirmed/Cancelled/Shipped）；Owner（Pending→Cancelled）
```

單一端點處理所有狀態轉換，由 service 驗證呼叫者權限與合法轉換。

## Risks / Trade-offs

- **Reserved stock 洩漏**：若程式 bug 導致訂單取消時未歸還 reserved，庫存將永久「被佔用」。緩解：取消路徑加入整合測試，未來可加排程清理孤立 reservation。
- **樂觀鎖衝突爆量**：在瞬間高並發時大量請求會失敗。緩解：回傳 409 讓 client 提示用戶重試；本平台為代購小批量場景，發生機率低。
- **Migration 不可逆**：`Status` 欄位從 string 轉換需要 data migration，確保既有 "Pending" 資料相容。緩解：EF migration 中加入 `Sql("UPDATE Orders SET Status = 'Pending' WHERE Status IS NULL")` 保護。

## Migration Plan

1. 加入 EF Core migration：`Products.ReservedStock`（int, default 0）、`Products.RowVersion`（rowversion）
2. `Orders.Status` 欄位型別不變（仍為 nvarchar），enum conversion 發生在應用層
3. 部署前執行 migration；若回滾只需移除欄位（ReservedStock / RowVersion），Status 欄位無需變更
