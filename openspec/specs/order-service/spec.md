## ADDED Requirements

### Requirement: 建立訂單時驗證商品存在
`OrderService` SHALL 在建立訂單前，對每筆 `OrderItemRequest` 查詢對應的 `Product`，若商品不存在或 `IsActive` 為 false，SHALL 拋出對應的 domain exception。

#### Scenario: 商品不存在
- **WHEN** `CreateAsync` 中某筆 item 的 `ProductId` 在資料庫不存在
- **THEN** SHALL 拋出 `ProductNotFoundException`

#### Scenario: 商品已下架
- **WHEN** 商品存在但 `IsActive` 為 false
- **THEN** SHALL 拋出 `InvalidProductDataException`（或對應下架例外）

### Requirement: 建立訂單時驗證庫存
`OrderService` SHALL 確認每筆 `OrderItem` 的 `Quantity` 不超過 `Product.Stock`，否則拋出例外。

#### Scenario: 庫存不足
- **WHEN** 某商品 `Stock` 為 2，但請求 `Quantity` 為 5
- **THEN** SHALL 拋出含有明確訊息的例外（例如 `InsufficientStockException`）

### Requirement: 建立訂單時快照 UnitPrice 並計算 TotalAmount
`OrderService` SHALL 以資料庫中的 `Product.Price` 作為 `OrderItem.UnitPrice`（忽略 Client 傳入的任何價格），並計算 `Order.TotalAmount = sum(Quantity × UnitPrice)`。

#### Scenario: UnitPrice 取自資料庫
- **WHEN** 成功建立訂單
- **THEN** 每筆 `OrderItem.UnitPrice` SHALL 等於建立當下 `Product.Price` 的值

#### Scenario: TotalAmount 正確計算
- **WHEN** 訂單含 2 筆明細（1×100 + 3×50）
- **THEN** `Order.TotalAmount` SHALL 為 250

### Requirement: 查詢當前使用者的所有訂單
`OrderService` SHALL 提供 `FindByUserIdAsync(string userId)` 方法，回傳該使用者的所有訂單（含明細）。

#### Scenario: 回傳使用者訂單列表
- **WHEN** 呼叫 `FindByUserIdAsync(userId)`
- **THEN** SHALL 回傳 `List<OrderResponse>`，只包含該 userId 的訂單

### Requirement: 查詢單筆訂單並驗證擁有權
`OrderService` SHALL 提供 `FindOneAsync(Guid id, string userId)` 方法，若訂單存在但不屬於該 userId，SHALL 回傳 null（由 Controller 轉換為 404）。

#### Scenario: 訂單屬於當前使用者
- **WHEN** 呼叫 `FindOneAsync(id, userId)`，訂單存在且 `UserId` 相符
- **THEN** SHALL 回傳 `OrderResponse`

#### Scenario: 訂單不存在或不屬於當前使用者
- **WHEN** 呼叫 `FindOneAsync(id, userId)`，訂單不存在或 `UserId` 不符
- **THEN** SHALL 回傳 null
