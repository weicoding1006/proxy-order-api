## ADDED Requirements

### Requirement: 建立產品端點
ProductController SHALL 提供 `POST /products` 端點，接收 `CreateProductDto`，呼叫 `ProductService.create()`，並回傳 `201 Created` 與 `ProductResponseDto`。

#### Scenario: 成功建立產品
- **WHEN** 用戶端發送有效的 `POST /products` 請求，body 包含 `name` 與 `price`
- **THEN** 系統回傳 HTTP 201，body 為新建產品的 `ProductResponseDto`

#### Scenario: 缺少必填欄位
- **WHEN** 用戶端發送 `POST /products`，body 缺少 `name` 欄位
- **THEN** 系統回傳 HTTP 400，並附上欄位驗證錯誤訊息

---

### Requirement: 查詢所有產品端點
ProductController SHALL 提供 `GET /products` 端點，呼叫 `ProductService.findAll()`，並回傳 `200 OK` 與 `ProductResponseDto[]`。

#### Scenario: 成功取得產品列表
- **WHEN** 用戶端發送 `GET /products`
- **THEN** 系統回傳 HTTP 200，body 為所有產品的陣列（可為空陣列）

---

### Requirement: 查詢單一產品端點
ProductController SHALL 提供 `GET /products/:id` 端點，呼叫 `ProductService.findOne(id)`，並回傳 `200 OK` 與 `ProductResponseDto`。

#### Scenario: 成功查詢存在的產品
- **WHEN** 用戶端發送 `GET /products/1`，且 ID 為 1 的產品存在
- **THEN** 系統回傳 HTTP 200，body 為對應產品的 `ProductResponseDto`

#### Scenario: 查詢不存在的產品
- **WHEN** 用戶端發送 `GET /products/999`，且 ID 為 999 的產品不存在
- **THEN** 系統回傳 HTTP 404，並附上錯誤訊息

---

### Requirement: 更新產品端點
ProductController SHALL 提供 `PATCH /products/:id` 端點，接收 `UpdateProductDto`，呼叫 `ProductService.update(id, dto)`，並回傳 `200 OK` 與更新後的 `ProductResponseDto`。

#### Scenario: 成功更新產品
- **WHEN** 用戶端發送有效的 `PATCH /products/1`，且該產品存在
- **THEN** 系統回傳 HTTP 200，body 為更新後的 `ProductResponseDto`

#### Scenario: 更新不存在的產品
- **WHEN** 用戶端發送 `PATCH /products/999`，且該產品不存在
- **THEN** 系統回傳 HTTP 404，並附上錯誤訊息

---

### Requirement: 刪除產品端點
ProductController SHALL 提供 `DELETE /products/:id` 端點，呼叫 `ProductService.remove(id)`，並回傳 `204 No Content`。

#### Scenario: 成功刪除產品
- **WHEN** 用戶端發送 `DELETE /products/1`，且該產品存在
- **THEN** 系統回傳 HTTP 204，無 body

#### Scenario: 刪除不存在的產品
- **WHEN** 用戶端發送 `DELETE /products/999`，且該產品不存在
- **THEN** 系統回傳 HTTP 404，並附上錯誤訊息

---

### Requirement: Controller 不含業務邏輯
ProductController SHALL 不包含任何業務驗證或資料轉換邏輯，所有業務決策 MUST 委派給 `ProductService`。

#### Scenario: Controller 僅作請求路由
- **WHEN** Controller 接收到任何請求
- **THEN** Controller 只負責解析請求參數並將其傳遞給 Service，再將 Service 回傳結果格式化為 HTTP 回應
