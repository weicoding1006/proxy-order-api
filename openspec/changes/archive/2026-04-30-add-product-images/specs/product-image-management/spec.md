## ADDED Requirements

### Requirement: 刪除商品圖片端點
ProductController SHALL 提供 `DELETE /api/products/{id}/images/{imageId}` 端點，刪除指定的 `ProductImage` 記錄並移除磁碟上的圖片檔案。

#### Scenario: 成功刪除圖片
- **WHEN** 用戶端發送 `DELETE /api/products/1/images/{imageId}`，且圖片存在且屬於該產品
- **THEN** 系統刪除資料庫記錄、移除磁碟檔案，回傳 HTTP 204 No Content

#### Scenario: 圖片不屬於指定產品
- **WHEN** 用戶端發送 `DELETE /api/products/1/images/{imageId}`，但該圖片屬於其他產品
- **THEN** 系統回傳 HTTP 404，不執行任何刪除

#### Scenario: 圖片不存在
- **WHEN** 用戶端發送 `DELETE /api/products/1/images/{不存在的imageId}`
- **THEN** 系統回傳 HTTP 404

#### Scenario: 刪除封面圖後自動指定新封面
- **WHEN** 被刪除的圖片是封面圖（IsCover = true），且該產品還有其他圖片
- **THEN** 系統自動將 SortOrder 最小的剩餘圖片設為新封面（IsCover = true）

#### Scenario: 刪除產品最後一張圖片
- **WHEN** 被刪除的圖片是該產品唯一的圖片
- **THEN** 系統刪除後，該產品的 images 陣列變為空陣列，回傳 HTTP 204

---

### Requirement: 設定封面圖端點
ProductController SHALL 提供 `PATCH /api/products/{id}/images/{imageId}/set-cover` 端點，將指定圖片設為封面圖。

#### Scenario: 成功設定封面圖
- **WHEN** 用戶端發送 `PATCH /api/products/1/images/{imageId}/set-cover`，且圖片存在且屬於該產品
- **THEN** 系統將該圖片的 `IsCover` 設為 `true`，同時將同產品其他所有圖片的 `IsCover` 設為 `false`，回傳 HTTP 200 與更新後的 `ProductImageDto`

#### Scenario: 圖片不屬於指定產品
- **WHEN** 用戶端試圖將其他產品的圖片設為封面
- **THEN** 系統回傳 HTTP 404，不執行任何更新

---

### Requirement: 更新圖片排序端點
ProductController SHALL 提供 `PATCH /api/products/{id}/images/{imageId}` 端點，允許更新 `SortOrder` 欄位。

#### Scenario: 成功更新排序
- **WHEN** 用戶端發送 `PATCH /api/products/1/images/{imageId}`，body 包含 `{ "sortOrder": 2 }`
- **THEN** 系統更新該圖片的 `SortOrder`，回傳 HTTP 200 與更新後的 `ProductImageDto`
