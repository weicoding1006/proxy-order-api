## Context

目前 `Product` Entity 只有基本商品欄位（Name、Price、Description、Stock 等），沒有任何圖片支援。代購平台需要展示商品照片讓買家判斷商品，且一個商品通常需要多張圖片（正面、側面、細節、尺寸對照等）。

圖片儲存策略的選擇直接影響到架構複雜度、部署成本與未來擴充性。

## Goals / Non-Goals

**Goals:**
- `ProductImage` Entity 支援一對多（一個產品多張圖片）
- 圖片儲存於本地 `wwwroot/uploads/products/{productId}/` 目錄，透過靜態 URL 存取
- 每張圖片記錄：URL、排序（SortOrder）、是否為封面（IsCover）、上傳時間
- API 支援：上傳圖片、刪除圖片、設定封面圖
- `ProductResponseDto` 包含圖片陣列

**Non-Goals:**
- 雲端儲存（S3、Azure Blob）—— 本階段使用本地磁碟，未來只需改 Service 寫入邏輯即可遷移
- 圖片壓縮、縮圖生成
- CDN 整合
- 圖片格式轉換

## Decisions

### 決策 1：圖片存本地磁碟，資料庫只存 URL 字串

**選擇**：將圖片檔案儲存於 `wwwroot/uploads/products/{productId}/`，資料庫的 `ImageUrl` 欄位只存相對路徑（如 `/uploads/products/abc/photo.jpg`）。

**原因**：
- 資料庫儲存 BLOB 會大幅增加 DB 負擔，備份體積龐大，查詢效能差
- 本地磁碟配合 `UseStaticFiles()` 是 ASP.NET Core 最簡單的靜態資源方案，無需額外套件
- 只存 URL 字串讓未來遷移 S3 時只需改 Service 寫入邏輯，資料庫 schema 與 API 回應格式完全不變

**取捨**：本地磁碟在多台伺服器水平擴展時會有同步問題，但代購平台初期單機部署，此問題暫不影響。

### 決策 2：`ProductImage` 作為獨立 Entity，透過 EF Core 導覽屬性關聯

**選擇**：建立獨立的 `ProductImage` Entity，並在 `Product` 上加入 `ICollection<ProductImage> Images` 導覽屬性。

**原因**：
- 獨立 Entity 可獨立增刪，不需每次都更新整個 Product
- EF Core 的 Cascade Delete 設定可確保刪除 Product 時自動清除所有關聯圖片記錄
- 導覽屬性讓查詢時可用 `.Include(p => p.Images)` 一次載入

**取捨**：每次查詢產品列表若都 Include 圖片，效能需注意，需確保用 eager loading 而非 lazy loading。

### 決策 3：封面圖用 `IsCover` 布林欄位

**選擇**：`ProductImage` 有 `bool IsCover` 欄位，同一 Product 只能有一張封面圖（透過 Service 層確保唯一性）。

**原因**：
- 在 `Product` 加外鍵 `CoverId` 會造成循環外鍵（Product → ProductImage → Product），增加 EF Core 設定複雜度
- `IsCover` 布林欄位更直觀，封面切換只需更新兩筆記錄

**取捨**：需在 Service 層手動維護唯一性（設定新封面前先清除舊封面的 IsCover）。

### 決策 4：使用 `IFormFile` 接收上傳，不引入額外套件

**選擇**：使用 ASP.NET Core 內建的 `IFormFile` 接收上傳，用 `Guid.NewGuid()` 產生不重複檔名。

**原因**：代購平台規模不需要複雜的媒體處理，內建功能已足夠，避免過度依賴外部套件。

## Risks / Trade-offs

- **本地磁碟遺失風險** → 確保 `wwwroot/uploads/` 加入備份策略，或未來遷移至 S3
- **大檔案上傳**：預設 `IFormFile` 有大小限制 → 在 Controller 加上檔案大小驗證（建議限制 5MB）
- **設定封面圖的 Race Condition**：多個請求同時呼叫 set-cover 可能造成多張封面 → 使用資料庫 Transaction 確保原子性
- **Product 被刪除時圖片檔案殘留**：EF Cascade Delete 只刪除資料庫記錄，磁碟檔案需在 Service 層手動清理

## Migration Plan

1. 建立 `ProductImage` Entity 與 `OrderDbContext` 設定
2. 執行 `dotnet ef migrations add AddProductImages`
3. 在 `Program.cs` 啟用 `UseStaticFiles()`，並確保 `wwwroot/uploads/products/` 目錄在啟動時自動建立
4. 部署後現有產品的 `Images` 陣列為空陣列，不影響現有功能

**回滾策略**：執行 `dotnet ef database update <上一個 Migration>`，移除新增的端點與 Service 方法。

**未來遷移 S3**：只需將 `ProductService` 中的寫檔邏輯改為呼叫 AWS SDK，`ImageUrl` 改存 S3 完整 URL，資料庫 schema 與所有 API 回應格式不需變動。
