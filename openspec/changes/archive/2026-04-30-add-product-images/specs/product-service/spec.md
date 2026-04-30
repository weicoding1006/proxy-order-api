## ADDED Requirements

### Requirement: 上傳圖片業務邏輯
ProductService SHALL 提供 `uploadImage(productId: Guid, file: IFormFile): ProductImageDto` 方法，驗證產品存在、儲存檔案至磁碟、新增 `ProductImage` 記錄，並回傳 `ProductImageDto`。

#### Scenario: 成功上傳第一張圖片（自動成為封面）
- **WHEN** 傳入存在的 `productId` 與有效的圖片檔案，且該產品目前無圖片
- **THEN** Service 儲存檔案至 `wwwroot/uploads/products/{productId}/`，建立 `IsCover = true` 的 `ProductImage`，回傳 `ProductImageDto`

#### Scenario: 成功上傳後續圖片
- **WHEN** 傳入存在的 `productId` 與有效的圖片檔案，且該產品已有封面圖
- **THEN** Service 儲存檔案，建立 `IsCover = false` 的 `ProductImage`，`SortOrder` 為目前最大值加一

#### Scenario: 產品不存在
- **WHEN** 傳入不存在的 `productId`
- **THEN** Service 拋出 `ProductNotFoundException`，不儲存任何檔案

#### Scenario: 檔案超過 5MB
- **WHEN** 傳入的 `IFormFile` 大小超過 5MB
- **THEN** Service 拋出驗證例外，不儲存任何檔案

---

### Requirement: 刪除圖片業務邏輯
ProductService SHALL 提供 `removeImage(productId: Guid, imageId: Guid): void` 方法，驗證圖片屬於該產品後，刪除資料庫記錄並移除磁碟檔案。

#### Scenario: 成功刪除非封面圖
- **WHEN** 傳入存在且屬於該產品的 `imageId`，且該圖片不是封面圖
- **THEN** Service 刪除資料庫記錄與磁碟檔案，完成後不回傳資料

#### Scenario: 刪除封面圖後自動指定新封面
- **WHEN** 被刪除的圖片是封面圖，且該產品還有其他圖片
- **THEN** Service 刪除後，將 `SortOrder` 最小的剩餘圖片設為新封面（IsCover = true）

#### Scenario: 圖片不屬於指定產品
- **WHEN** 傳入的 `imageId` 存在但屬於其他產品
- **THEN** Service 拋出 `ProductImageNotFoundException`

---

### Requirement: 設定封面圖業務邏輯
ProductService SHALL 提供 `setCoverImage(productId: Guid, imageId: Guid): ProductImageDto` 方法，在同一 Transaction 中將所有同產品圖片的 `IsCover` 設為 `false`，再將目標圖片設為 `true`。

#### Scenario: 成功設定封面圖
- **WHEN** 傳入存在且屬於該產品的 `imageId`
- **THEN** Service 在 Transaction 中更新所有圖片的 `IsCover` 狀態，回傳更新後的 `ProductImageDto`

#### Scenario: 圖片不屬於指定產品
- **WHEN** 傳入的 `imageId` 不屬於該產品
- **THEN** Service 拋出 `ProductImageNotFoundException`，不執行任何更新

---

### Requirement: 查詢產品時包含圖片
ProductService SHALL 在 `findAll()` 與 `findOne()` 方法中，一併載入產品的 `Images` 集合，依 `SortOrder` 升冪排列後映射至 `ProductResponseDto.images`。

#### Scenario: 查詢有圖片的產品
- **WHEN** 呼叫 `findOne(id)`，且該產品有 3 張圖片
- **THEN** 回傳的 `ProductResponseDto.images` 包含 3 個 `ProductImageDto`，依 `SortOrder` 升冪排列

#### Scenario: 查詢無圖片的產品
- **WHEN** 呼叫 `findOne(id)`，且該產品無任何圖片
- **THEN** 回傳的 `ProductResponseDto.images` 為空陣列 `[]`
