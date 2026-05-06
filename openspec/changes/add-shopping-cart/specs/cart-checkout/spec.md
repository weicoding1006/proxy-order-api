## ADDED Requirements

### Requirement: 購物車結帳建立訂單
`CartService` SHALL 提供 `CheckoutAsync(string userId)` 方法，將購物車轉換為正式 `Order`：
1. 取得使用者購物車（含 CartItems）
2. 若購物車為空，拋出 `CartEmptyException`（回傳 400）
3. 組裝 `CreateOrderRequest`，以各 CartItem 的 ProductId 與 Quantity 填入 OrderItemRequests
4. 呼叫既有 `OrderService.CreateAsync`（複用庫存驗證、UnitPrice 快照、ReservedStock 邏輯）
5. 成功後清空購物車（刪除所有 CartItems）
6. 回傳新建立的 `OrderResponse`

#### Scenario: 正常結帳
- **WHEN** 使用者 `POST /api/cart/checkout`，購物車含有效商品且庫存充足
- **THEN** 建立 Order 與 OrderItems，清空購物車，回傳 201 Created 含 OrderResponse

#### Scenario: 購物車為空
- **WHEN** 使用者 `POST /api/cart/checkout`，購物車沒有任何商品
- **THEN** 回傳 400 Bad Request，訊息說明購物車為空

#### Scenario: 某商品庫存不足
- **WHEN** 結帳時某商品 Stock 不足 CartItem.Quantity
- **THEN** 回傳 400 Bad Request（由 OrderService 拋出），購物車保持不變（不清空）

#### Scenario: 某商品已下架
- **WHEN** 結帳時購物車內某商品 IsActive = false
- **THEN** 回傳 400 Bad Request，購物車保持不變

### Requirement: 結帳 API 端點
系統 SHALL 提供 `POST /api/cart/checkout` 端點，需要 `[Authorize]` 驗證，僅限已登入使用者呼叫。

#### Scenario: 未登入使用者呼叫
- **WHEN** 未附帶有效 JWT Token 呼叫 `POST /api/cart/checkout`
- **THEN** 回傳 401 Unauthorized

#### Scenario: 成功結帳回傳訂單
- **WHEN** 已登入使用者呼叫且購物車有效
- **THEN** 回傳 201 Created，Body 為 `OrderResponse`（含 OrderId、TotalAmount、OrderItems 等）
