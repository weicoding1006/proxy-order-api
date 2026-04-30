## ADDED Requirements

### Requirement: 上傳商品圖片端點
ProductController SHALL 提供 `POST /api/products/{id}/images` 端點，接收 `multipart/form-data`，呼叫 `ProductService.uploadImage()`，並回傳 `201 Created` 與 `ProductImageDto`。

#### Scenario: 成功上傳
- **WHEN** 用戶端發送有效的 `POST /api/products/1/images` 含圖片檔案
- **THEN** Controller 將 `IFormFile` 與 `productId` 傳給 Service，回傳 HTTP 201 與新圖片的 `ProductImageDto`

#### Scenario: 產品不存在
- **WHEN** Service 拋出 `ProductNotFoundException`
- **THEN** Controller 回傳 HTTP 404

---

### Requirement: 刪除商品圖片端點
ProductController SHALL 提供 `DELETE /api/products/{id}/images/{imageId}` 端點，呼叫 `ProductService.removeImage()`，並回傳 `204 No Content`。

#### Scenario: 成功刪除
- **WHEN** 用戶端發送 `DELETE /api/products/1/images/{imageId}`，且圖片存在
- **THEN** Controller 呼叫 Service，回傳 HTTP 204，無 body

#### Scenario: 圖片不存在或不屬於該產品
- **WHEN** Service 拋出 `ProductImageNotFoundException`
- **THEN** Controller 回傳 HTTP 404

---

### Requirement: 設定封面圖端點
ProductController SHALL 提供 `PATCH /api/products/{id}/images/{imageId}/set-cover` 端點，呼叫 `ProductService.setCoverImage()`，並回傳 `200 OK` 與更新後的 `ProductImageDto`。

#### Scenario: 成功設定封面
- **WHEN** 用戶端發送 `PATCH /api/products/1/images/{imageId}/set-cover`
- **THEN** Controller 呼叫 Service，回傳 HTTP 200 與更新後的 `ProductImageDto`

---

### Requirement: Controller 不含業務邏輯
ProductController SHALL 不包含任何圖片業務驗證或資料轉換邏輯，所有業務決策 MUST 委派給 `ProductService`。

#### Scenario: Controller 僅作請求路由
- **WHEN** Controller 接收到圖片相關請求
- **THEN** Controller 只負責解析請求參數並將其傳遞給 Service，再將 Service 回傳結果格式化為 HTTP 回應
