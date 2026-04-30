## Why

目前 `Product` 實體不包含任何圖片資訊，代購平台的商品頁面無法展示商品照片，導致使用者無法直觀判斷商品樣式。由於一個商品通常需要多角度照片（封面、細節、尺寸對照），因此需要支援一對多的商品圖片關係。

## What Changes

- 新增 `ProductImage` Entity，與 `Product` 建立一對多關聯
- 圖片檔案儲存於伺服器本地磁碟（`wwwroot/uploads/products/`），透過靜態檔案 URL 存取
- 新增上傳圖片端點：`POST /api/products/{id}/images`（multipart/form-data）
- 新增刪除圖片端點：`DELETE /api/products/{id}/images/{imageId}`
- 新增設定封面圖端點：`PATCH /api/products/{id}/images/{imageId}/set-cover`
- `ProductResponseDto` 擴充包含 `images` 陣列（含封面圖標記）
- 新增 EF Core Migration 建立 `ProductImages` 資料表

## Capabilities

### New Capabilities

- `product-image-upload`: 上傳商品圖片至伺服器，支援一個商品多張圖片，並記錄圖片 URL、排序、是否為封面
- `product-image-management`: 刪除指定圖片、設定封面圖的管理操作

### Modified Capabilities

- `product-dto`: `ProductResponseDto` 需新增 `images: ProductImageDto[]` 欄位
- `product-controller`: 新增三個圖片相關端點
- `product-service`: 新增圖片上傳、刪除、封面設定的業務邏輯
- `product-repository`: 新增圖片相關的資料存取方法
- `proxy-platform-data-model`: 新增 `ProductImage` Entity 與資料庫對應設定

## Impact

- **新檔案**：`ProductImage` Entity、`ProductImageDto`、圖片 Service 方法、Migration
- **修改檔案**：`Product` Entity（新增導覽屬性）、`ProductResponseDto`、`ProductController`、`ProductService`、`OrderDbContext`
- **靜態檔案**：需在 `Program.cs` 啟用 `UseStaticFiles()`，並確保 `wwwroot/uploads/products/` 目錄存在
- **依賴**：不需引入外部套件，使用 ASP.NET Core 內建的 `IFormFile` 處理上傳
