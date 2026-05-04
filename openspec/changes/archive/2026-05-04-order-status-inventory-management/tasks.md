## 1. Domain Layer

- [x] 1.1 建立 `OrderStatus` enum（`Pending`, `Confirmed`, `Shipped`, `Completed`, `Cancelled`）於 `OrderSystem.Domain`
- [x] 1.2 將 `Order.Status` 型別從 `string` 改為 `OrderStatus`，預設值改為 `OrderStatus.Pending`
- [x] 1.3 在 `Product` 加入 `ReservedStock` 屬性（`int`, 預設 0）
- [x] 1.4 在 `Product` 加入 `RowVersion` 屬性（`byte[]`，用於 EF Core Optimistic Concurrency）
- [x] 1.5 建立 `InvalidOrderStatusTransitionException` 例外類別

## 2. Application Layer — 庫存保留邏輯

- [x] 2.1 在 `IProductRepository` 加入 `UpdateReservedStockAsync(Guid productId, int delta)` 方法
- [x] 2.2 在 `IOrderRepository` 加入 `UpdateStatusAsync(Guid orderId, OrderStatus newStatus)` 方法
- [x] 2.3 修改 `OrderService.CreateAsync`：庫存檢查改為 `Stock - ReservedStock >= quantity`，通過後呼叫 `UpdateReservedStockAsync` 增加 reservation
- [x] 2.4 在 `OrderService` 加入 `UpdateStatusAsync(Guid orderId, OrderStatus newStatus, string callerId, bool isAdmin)` 方法，包含：
  - 驗證合法狀態轉換（對照 design.md 轉換表）
  - 驗證呼叫者權限（Owner 只能取消自己的 Pending 訂單）
  - `Pending → Confirmed`：呼叫 `UpdateReservedStockAsync(-Q)` 並 `stock -= Q`
  - `Pending → Cancelled`：呼叫 `UpdateReservedStockAsync(-Q)` 歸還 reserved
  - `Confirmed → Cancelled`：呼叫 `stock += Q` 歸還已扣庫存

## 3. Infrastructure Layer

- [x] 3.1 在 EF Core `DbContext` 設定 `Order.Status` 使用 `HasConversion<string>()` 存為字串
- [x] 3.2 在 EF Core `DbContext` 設定 `Product.RowVersion` 為 `IsRowVersion()`（concurrency token）
- [x] 3.3 實作 `IProductRepository.UpdateReservedStockAsync`，捕捉 `DbUpdateConcurrencyException` 並重新拋出 `InsufficientStockException`
- [x] 3.4 實作 `IOrderRepository.UpdateStatusAsync`
- [x] 3.5 新增 EF Core Migration：加入 `Products.ReservedStock`（int, default 0）與 `Products.RowVersion`（rowversion）

## 4. API Layer

- [x] 4.1 建立 `UpdateOrderStatusRequest` DTO（包含 `OrderStatus Status` 屬性）
- [x] 4.2 在 `OrdersController` 加入 `PATCH /api/orders/{id}/status` 端點（需 `[Authorize]`）
- [x] 4.3 端點邏輯：從 JWT 取得 `userId` 與 `isAdmin`，呼叫 `OrderService.UpdateStatusAsync`，回傳更新後的 `OrderResponse`
- [x] 4.4 處理例外：`InvalidOrderStatusTransitionException` → 422，`UnauthorizedAccessException` → 403，`OrderNotFoundException` → 404

## 5. 驗證

- [ ] 5.1 手動測試：建立訂單 → 確認庫存 ReservedStock 增加、Stock 不變
- [ ] 5.2 手動測試：Admin 確認訂單 → ReservedStock 減少、Stock 減少
- [ ] 5.3 手動測試：取消 Pending 訂單 → ReservedStock 歸還
- [ ] 5.4 手動測試：取消 Confirmed 訂單 → Stock 歸還
- [ ] 5.5 測試非法狀態轉換（如 Pending → Shipped）回傳 422
- [ ] 5.6 測試非 Admin 嘗試 Admin-only 操作回傳 403
