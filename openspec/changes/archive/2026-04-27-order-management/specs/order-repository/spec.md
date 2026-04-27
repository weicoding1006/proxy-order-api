## ADDED Requirements

### Requirement: IOrderRepository 介面定義
Application 層 SHALL 定義 `IOrderRepository` 介面，宣告訂單資料存取的所有方法，使 Service 層不直接依賴 Infrastructure 實作。

#### Scenario: 介面方法簽章
- **WHEN** 定義 `IOrderRepository`
- **THEN** SHALL 包含以下方法：
  - `Task<Order> CreateAsync(Order order)`
  - `Task<List<Order>> FindByUserIdAsync(string userId)`
  - `Task<Order?> FindByIdAsync(Guid id)`

### Requirement: OrderRepository 實作
Infrastructure 層 SHALL 提供 `OrderRepository` 類別實作 `IOrderRepository`，注入 `OrderDbContext`，並以 primary constructor 風格宣告（與 `ProductRepository` 一致）。

#### Scenario: CreateAsync 儲存訂單與明細
- **WHEN** 呼叫 `CreateAsync(order)`，order 包含多個 `OrderItem`
- **THEN** `Orders` 與 `OrderItems` 資料表 SHALL 各新增對應紀錄
- **AND** SHALL 回傳已儲存的 `Order` 物件（含資料庫產生的 Id）

#### Scenario: FindByUserIdAsync 只回傳該使用者的訂單
- **WHEN** 呼叫 `FindByUserIdAsync(userId)`
- **THEN** SHALL 回傳 `UserId` 等於傳入值的所有 `Order`，並 Include `OrderItems`
- **AND** SHALL NOT 回傳其他使用者的訂單

#### Scenario: FindByIdAsync 找不到時回傳 null
- **WHEN** 呼叫 `FindByIdAsync(id)`，該 id 不存在於資料庫
- **THEN** SHALL 回傳 `null`

#### Scenario: FindByIdAsync 找到時 Include OrderItems
- **WHEN** 呼叫 `FindByIdAsync(id)`，該 id 存在
- **THEN** 回傳的 `Order` SHALL 包含已載入的 `OrderItems` 集合
