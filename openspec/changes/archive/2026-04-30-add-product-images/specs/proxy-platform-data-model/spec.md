## ADDED Requirements

### Requirement: ProductImage Entity 定義
系統需有一個 `ProductImage` 實體，記錄商品圖片的 URL 與展示資訊。

#### Scenario: ProductImage 屬性定義
- **WHEN** 建立 ProductImage 實體
- **THEN** 必須包含 `Id (Guid)`、`ProductId (Guid)`、`ImageUrl (string)`、`IsCover (bool)`、`SortOrder (int)`、`CreatedAt (DateTime)` 等屬性

---

### Requirement: Product 與 ProductImage 一對多關聯
`Product` Entity SHALL 包含 `ICollection<ProductImage> Images` 導覽屬性，`ProductImage` MUST 包含 `ProductId` 外鍵與 `Product` 反向導覽屬性。

#### Scenario: EF Core Cascade Delete 設定
- **WHEN** 刪除 `Product` 記錄時
- **THEN** EF Core 自動刪除所有關聯的 `ProductImage` 記錄（Cascade Delete）

#### Scenario: 查詢時可用 Include 載入圖片
- **WHEN** 使用 `dbContext.Products.Include(p => p.Images)` 查詢
- **THEN** 回傳的 `Product` 物件的 `Images` 集合已填充完整資料

---

### Requirement: ProductImages 資料表設定
`OrderDbContext` SHALL 在 `OnModelCreating` 中設定 `ProductImage` Entity：
- `ImageUrl` 欄位最大長度 500 字元、非 null
- `SortOrder` 欄位有預設值 0
- `IsCover` 欄位有預設值 false
- `CreatedAt` 欄位有預設值 `GETUTCDATE()`（資料庫層級）

#### Scenario: Migration 產生正確資料表
- **WHEN** 執行 `dotnet ef migrations add AddProductImages` 後套用 Migration
- **THEN** 資料庫中新增 `ProductImages` 資料表，含 `ProductId` 外鍵索引與 Cascade Delete 約束
