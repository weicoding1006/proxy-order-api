## MODIFIED Requirements

### Requirement: 查詢當前使用者的所有訂單
`OrderService` SHALL 提供 `FindByUserIdAsync(string userId)` 方法，回傳該使用者的所有訂單（含明細）。此需求無變更，列於此以標示 `order-management` 規格與購物車結帳流程的整合點。

#### Scenario: 回傳使用者訂單列表
- **WHEN** 呼叫 `FindByUserIdAsync(userId)`
- **THEN** SHALL 回傳 `List<OrderResponse>`，只包含該 userId 的訂單，包含從購物車結帳建立的訂單

### Requirement: 建立訂單時驗證商品存在
`OrderService` SHALL 在建立訂單前，對每筆 `OrderItemRequest` 查詢對應的 `Product`，若商品不存在或 `IsActive` 為 false，SHALL 拋出對應的 domain exception。此邏輯由購物車結帳路徑（`CartService.CheckoutAsync`）與直接下單路徑共同複用。

#### Scenario: 商品不存在
- **WHEN** `CreateAsync` 中某筆 item 的 `ProductId` 在資料庫不存在
- **THEN** SHALL 拋出 `ProductNotFoundException`

#### Scenario: 商品已下架
- **WHEN** 商品存在但 `IsActive` 為 false
- **THEN** SHALL 拋出 `InvalidProductDataException`（或對應下架例外）
