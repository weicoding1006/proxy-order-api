## ADDED Requirements

### Requirement: GET /api/orders/all 僅限 Admin 存取
`OrdersController` SHALL 提供 `GET /api/orders/all` 端點，且僅允許具備 `Admin` 角色的已驗證使用者呼叫，回傳平台上所有訂單。

#### Scenario: Admin 成功取得所有訂單
- **WHEN** 具有 `Admin` 角色的已登入使用者 GET `/api/orders/all`
- **THEN** 回應 SHALL 為 `200 OK`，body 為 `List<OrderResponse>`，包含所有使用者的訂單

#### Scenario: 非 Admin 已登入使用者回傳 403
- **WHEN** 已登入但不具有 `Admin` 角色的使用者 GET `/api/orders/all`
- **THEN** 回應 SHALL 為 `403 Forbidden`

#### Scenario: 未登入使用者回傳 401
- **WHEN** 未附 `Authorization: Bearer <token>` 標頭呼叫 `GET /api/orders/all`
- **THEN** 回應 SHALL 為 `401 Unauthorized`
