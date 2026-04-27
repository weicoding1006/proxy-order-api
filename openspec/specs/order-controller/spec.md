## ADDED Requirements

### Requirement: POST /api/orders 建立訂單
`OrdersController` SHALL 提供 `POST /api/orders` 端點，需要有效的 JWT Bearer Token，從 token claims 取得 UserId，呼叫 `OrderService.CreateAsync`，成功後回傳 `201 Created`。

#### Scenario: 成功建立訂單
- **WHEN** 已登入使用者 POST `/api/orders`，body 含有效的 `Items`（存在商品、庫存充足）
- **THEN** 回應 SHALL 為 `201 Created`
- **AND** response body SHALL 包含完整 `OrderResponse`（含 Items、TotalAmount）

#### Scenario: 未帶 Token 回傳 401
- **WHEN** 未附 `Authorization: Bearer <token>` 標頭呼叫 `POST /api/orders`
- **THEN** 回應 SHALL 為 `401 Unauthorized`

#### Scenario: 商品不存在回傳 404
- **WHEN** 請求中包含不存在的 `ProductId`
- **THEN** 回應 SHALL 為 `404 Not Found`，並附帶錯誤訊息

#### Scenario: 庫存不足回傳 400
- **WHEN** 請求中某商品的 `Quantity` 超過 `Stock`
- **THEN** 回應 SHALL 為 `400 Bad Request`，並附帶庫存不足的明確訊息

### Requirement: GET /api/orders 查詢當前使用者訂單列表
`OrdersController` SHALL 提供 `GET /api/orders` 端點，需要 JWT 認證，回傳當前使用者的所有訂單。

#### Scenario: 成功取得訂單列表
- **WHEN** 已登入使用者 GET `/api/orders`
- **THEN** 回應 SHALL 為 `200 OK`，body 為 `List<OrderResponse>`，只含自己的訂單

#### Scenario: 尚無訂單時回傳空陣列
- **WHEN** 已登入使用者尚未建立過任何訂單
- **THEN** 回應 SHALL 為 `200 OK`，body 為 `[]`

### Requirement: GET /api/orders/{id} 查詢單筆訂單
`OrdersController` SHALL 提供 `GET /api/orders/{id}` 端點，需要 JWT 認證，若訂單不存在或不屬於當前使用者，回傳 `404 Not Found`。

#### Scenario: 成功取得自己的訂單
- **WHEN** 已登入使用者 GET `/api/orders/{id}`，該訂單屬於自己
- **THEN** 回應 SHALL 為 `200 OK`，body 為 `OrderResponse`

#### Scenario: 訂單不屬於當前使用者回傳 404
- **WHEN** 已登入使用者 GET `/api/orders/{id}`，該訂單屬於其他使用者
- **THEN** 回應 SHALL 為 `404 Not Found`（不洩漏訂單是否存在）

#### Scenario: 訂單 id 不存在回傳 404
- **WHEN** 已登入使用者 GET `/api/orders/{id}`，id 在資料庫不存在
- **THEN** 回應 SHALL 為 `404 Not Found`
