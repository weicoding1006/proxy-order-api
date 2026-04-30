## ADDED Requirements

### Requirement: 上傳商品圖片端點
ProductController SHALL 提供 `POST /api/products/{id}/images` 端點，接收 `multipart/form-data` 格式的圖片檔案，將圖片儲存至本地磁碟並新增 `ProductImage` 記錄。

#### Scenario: 成功上傳圖片
- **WHEN** 用戶端發送有效的 `POST /api/products/1/images`，包含一個圖片檔案（JPEG 或 PNG，不超過 5MB）
- **THEN** 系統將檔案儲存至 `wwwroot/uploads/products/{productId}/`，在資料庫新增 `ProductImage` 記錄，並回傳 `201 Created` 與新圖片的 `ProductImageDto`

#### Scenario: 產品不存在時上傳圖片
- **WHEN** 用戶端發送 `POST /api/products/999/images`，且 ID 為 999 的產品不存在
- **THEN** 系統回傳 HTTP 404，不儲存任何檔案

#### Scenario: 檔案超過大小限制
- **WHEN** 用戶端發送的圖片檔案超過 5MB
- **THEN** 系統回傳 HTTP 400，附上錯誤訊息說明大小限制

#### Scenario: 未提供檔案
- **WHEN** 用戶端發送 `POST /api/products/1/images`，但未包含任何檔案
- **THEN** 系統回傳 HTTP 400，附上欄位驗證錯誤訊息

---

### Requirement: 第一張上傳的圖片自動設為封面
ProductService SHALL 在圖片上傳時，若該產品目前沒有任何圖片，自動將第一張圖片的 `IsCover` 設為 `true`。

#### Scenario: 上傳第一張圖片自動成為封面
- **WHEN** 產品目前無任何圖片，用戶端上傳第一張圖片
- **THEN** 新圖片的 `IsCover = true`，`ProductImageDto.isCover` 回傳 `true`

#### Scenario: 上傳後續圖片不覆蓋封面
- **WHEN** 產品已有封面圖，用戶端再次上傳圖片
- **THEN** 新圖片的 `IsCover = false`，原封面圖保持不變

---

### Requirement: ProductImageDto 定義
系統 SHALL 定義 `ProductImageDto`，包含以下欄位（映射自 `ProductImage` Entity）：
- `id: Guid`
- `imageUrl: string`（可直接用於 `<img src>` 的相對路徑）
- `isCover: bool`
- `sortOrder: int`
- `createdAt: DateTime`

#### Scenario: DTO 隔離 Entity 細節
- **WHEN** Controller 回傳圖片資訊
- **THEN** 回傳的是 `ProductImageDto`，不包含 `ProductId` 外鍵或其他資料庫內部欄位
