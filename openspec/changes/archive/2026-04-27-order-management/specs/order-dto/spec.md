## ADDED Requirements

### Requirement: CreateOrderRequest DTO
Application 層 SHALL 定義 `CreateOrderRequest`，代表使用者建立訂單的請求資料。

#### Scenario: CreateOrderRequest 欄位
- **WHEN** Client 送出建立訂單請求
- **THEN** request body SHALL 包含 `Items: List<OrderItemRequest>`
- **AND** `OrderItemRequest` SHALL 包含 `ProductId: Guid` 與 `Quantity: int`
- **AND** `Items` SHALL NOT 為 null 或空集合（至少一筆明細）
- **AND** `Quantity` SHALL 大於 0

### Requirement: OrderItemResponse DTO
Application 層 SHALL 定義 `OrderItemResponse`，代表訂單明細的回應資料。

#### Scenario: OrderItemResponse 欄位
- **WHEN** 回傳訂單明細
- **THEN** SHALL 包含 `Id: Guid`、`ProductId: Guid`、`Quantity: int`、`UnitPrice: decimal`

### Requirement: OrderResponse DTO
Application 層 SHALL 定義 `OrderResponse`，代表訂單主檔的回應資料，含靜態工廠方法 `FromEntity`。

#### Scenario: OrderResponse 欄位
- **WHEN** 回傳訂單
- **THEN** SHALL 包含 `Id: Guid`、`UserId: string`、`TotalAmount: decimal`、`Status: string`、`CreatedAt: DateTime`、`Items: List<OrderItemResponse>`

#### Scenario: OrderResponse.FromEntity 映射
- **WHEN** 呼叫 `OrderResponse.FromEntity(order)`
- **THEN** SHALL 正確映射所有欄位，包含 `Items` 集合的每筆 `OrderItemResponse`
