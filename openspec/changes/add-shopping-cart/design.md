## Context

目前系統已有完整的 Order / OrderItem / Product / ApplicationUser 實體與訂單建立流程。使用者下單時直接建立訂單，沒有中間緩衝層。加入購物車功能需要新增兩個實體（`Cart`、`CartItem`），並擴充 `ApplicationUser` 的導覽屬性，同時新增對應的 Repository、Service 與 API 端點。

## Goals / Non-Goals

**Goals:**
- 新增 `Cart` 與 `CartItem` Domain 實體及 EF Core 設定
- 提供購物車 CRUD API（加入、更新數量、移除、清空、查詢）
- 提供「購物車結帳」API，從 Cart 建立 Order 並清空購物車
- 維持現有直接建立訂單的 API 不受影響

**Non-Goals:**
- 購物車持久化以外的暫存機制（不使用 Session / Redis）
- 訪客（未登入）購物車
- 購物車合併（多裝置同步）

## Decisions

### 1. 一人一車（Cart 與 ApplicationUser 一對一）
每位使用者在資料庫只有一筆 `Cart`，以 `UserId` 為 UNIQUE KEY。首次 `GET /api/cart` 時若不存在則自動建立。
**替代方案**：一人多車（如多份願望清單）→ 本平台不需要，複雜度不必要。

### 2. CartItem 以 (CartId + ProductId) 做複合 UNIQUE
避免同一商品在購物車重複出現；加入已存在的商品時改為累加數量（Upsert 語意）。
**替代方案**：允許重複列，由 Application 層合併 → 資料不一致風險高。

### 3. Cart 不儲存金額，結帳時即時計算
CartItem 只存 `ProductId` 與 `Quantity`，不快照價格。結帳（checkout）時才讀取 `Product.Price` 並快照至 `OrderItem.UnitPrice`，沿用既有 OrderService 邏輯。
**替代方案**：CartItem 儲存快照價格 → 購物車只是暫存意圖，尚未付款，不需快照，保持簡單。

### 4. Checkout 複用 OrderService.CreateAsync
`CartService.CheckoutAsync` 組裝 `CreateOrderRequest`（含 OrderItemRequests），直接呼叫既有 `OrderService.CreateAsync`，確保庫存驗證、UnitPrice 快照、ReservedStock 等邏輯不重複實作。

### 5. Cart 實體放在 Domain/Entities，遵循現有架構
不引入新的分層或 DDD Aggregate，與現有 `Order` / `Product` 實體風格一致。

## Risks / Trade-offs

- **並發加入相同商品** → CartItem UNIQUE 約束會在資料庫層拋出衝突例外，Application 層應捕捉並轉換為友善訊息（或使用 Upsert 避免例外）
- **結帳時庫存不足** → 由 OrderService 現有驗證邏輯處理，回傳 400 Bad Request
- **購物車資料無過期機制** → 長期閒置的購物車會留在資料庫；初期可接受，未來可加 TTL 清理 Job
- **CartItem 不快照價格** → 若商品改價，使用者可能對結帳金額感到意外；結帳前 GET /api/cart 應回傳當下即時價格讓使用者確認

## Migration Plan

1. 新增 `Cart` / `CartItem` Domain 實體
2. 修改 `ApplicationUser` 加入 `Cart?` 導覽屬性
3. 在 `OrderDbContext.OnModelCreating` 設定兩張資料表的 UNIQUE 索引與關聯
4. 執行 `dotnet ef migrations add AddShoppingCart`
5. 套用 Migration（`dotnet ef database update`）
6. 實作 `ICartRepository` / `CartRepository`
7. 實作 `CartService`（含 checkout 邏輯）
8. 新增 `CartController`
9. 在 DI 容器（`Program.cs`）註冊新服務

**Rollback**：執行 `dotnet ef database update <前一個 Migration 名稱>` 後刪除 Migration 檔案。

## Open Questions

- 購物車商品數量上限是否需要限制（如單項 ≤ 99）？
- 結帳後是否需要寄送確認通知？（與現有訂單建立流程相同即可）
