## Why

目前訂單建立後直接以 `Pending` 狀態存入，但沒有對應的庫存扣減機制——若多筆訂單同時送出，相同商品可能被超賣。需要定義明確的庫存扣減時機與訂單狀態流程，確保庫存一致性並提供管理員審核的彈性空間。

## What Changes

- 在 `Order` 實體中以強型別 `enum` 取代 `string` 表示訂單狀態（`Pending → Confirmed → Shipped → Completed / Cancelled`）
- 訂單建立（`Pending`）時改採「軟性保留」：新增 `reserved_stock` 欄位於 `Product`，以原子操作保留庫存而非直接扣減
- 管理員確認訂單（`Pending → Confirmed`）時正式扣減 `stock` 並釋放 `reserved_stock`
- 訂單取消（任意狀態 → `Cancelled`）時歸還保留或已扣減的庫存
- 新增 `PATCH /api/orders/{id}/status` 端點，供管理員更新訂單狀態
- 庫存檢查改為同時驗證 `stock - reserved_stock >= requestedQuantity`，避免超賣

## Capabilities

### New Capabilities

- `order-status-transition`: 定義訂單狀態機（Pending / Confirmed / Shipped / Completed / Cancelled）及合法的狀態轉換規則
- `inventory-reservation`: 建立庫存保留（reserved stock）機制，訂單建立時軟性佔用庫存，確認時正式扣減，取消時歸還

### Modified Capabilities

- （無現有 spec 需要修改）

## Impact

- **Domain**: `Order`（Status 改 enum）、`Product`（新增 `ReservedStock`）、`OrderItem`（不變）
- **Application**: `OrderService`（建立、取消、確認邏輯）；新增 `UpdateOrderStatusAsync`
- **Infrastructure**: EF Core migration 更新 `Orders.Status` 與 `Products.ReservedStock`
- **API**: 新增 `PATCH /api/orders/{id}/status`（Admin only）
- **Breaking**: `Order.Status` 型別從 `string` 改為 `OrderStatus` enum，回應 JSON 值不變（仍為字串）
