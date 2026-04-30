## 1. Domain Entity

- [x] 1.1 在 `OrderSystem.Domain/Entities/` 新增 `ProductImage.cs`，包含 `Id (Guid)`、`ProductId (Guid)`、`ImageUrl (string)`、`IsCover (bool)`、`SortOrder (int)`、`CreatedAt (DateTime)` 欄位
- [x] 1.2 在 `Product.cs` 新增 `ICollection<ProductImage> Images` 導覽屬性（初始化為空集合）
- [x] 1.3 新增 `ProductImageNotFoundException` 例外類別至 `OrderSystem.Domain/Exceptions/`

## 2. DbContext 與 Migration

- [x] 2.1 在 `OrderDbContext.cs` 新增 `DbSet<ProductImage> ProductImages`
- [x] 2.2 在 `OrderDbContext.OnModelCreating` 設定 `ProductImage` Entity：`ImageUrl` 最大長度 500、`SortOrder` 預設值 0、`IsCover` 預設值 false、Cascade Delete、`CreatedAt` 預設值 `DateTime.UtcNow`
- [x] 2.3 執行 `dotnet ef migrations add AddProductImages --project OrderSystem.Infrastructure --startup-project OrderSystem.Api` 產生 Migration
- [x] 2.4 執行 `dotnet ef database update` 套用 Migration

## 3. 靜態檔案設定

- [x] 3.1 在 `Program.cs` 確認已啟用 `app.UseStaticFiles()`
- [x] 3.2 在 `Program.cs` 的啟動邏輯中，確保 `wwwroot/uploads/products/` 目錄存在（使用 `Directory.CreateDirectory`）

## 4. DTO

- [x] 4.1 在 `OrderSystem.Application/DTOs/` 新增 `ProductImageDto.cs`，包含 `Id`、`ImageUrl`、`IsCover`、`SortOrder`、`CreatedAt`
- [x] 4.2 在 `ProductResponseDto.cs` 新增 `List<ProductImageDto> Images` 欄位（預設空集合）

## 5. Repository

- [x] 5.1 在 `IProductRepository.cs` 新增方法：`AddImageAsync`、`FindImageByIdAsync`、`RemoveImageAsync`
- [x] 5.2 修改 `IProductRepository.cs` 的 `FindAllAsync` 與 `FindByIdAsync` 方法簽章，確認包含圖片集合
- [x] 5.3 在 `ProductRepository.cs` 實作新增的介面方法
- [x] 5.4 修改 `ProductRepository.cs` 的 `FindAllAsync` 與 `FindByIdAsync`，加入 `.Include(p => p.Images.OrderBy(i => i.SortOrder))` eager loading

## 6. Service

- [x] 6.1 在 `ProductService.cs` 新增 `UploadImageAsync(Guid productId, IFormFile file): Task<ProductImageDto>` 方法（含檔案大小 5MB 驗證、第一張自動設封面邏輯）
- [x] 6.2 在 `ProductService.cs` 新增 `RemoveImageAsync(Guid productId, Guid imageId): Task` 方法（含刪除磁碟檔案、封面繼承邏輯）
- [x] 6.3 在 `ProductService.cs` 新增 `SetCoverImageAsync(Guid productId, Guid imageId): Task<ProductImageDto>` 方法（含 Transaction、清除舊封面邏輯）
- [x] 6.4 修改 `ProductService.cs` 的 `FindAllAsync` 與 `FindOneAsync`，將 `Product.Images` 映射至 `ProductResponseDto.images`

## 7. Controller

- [x] 7.1 在 `ProductController.cs` 新增 `POST /api/products/{id}/images` 端點，接收 `IFormFile file`，呼叫 `ProductService.UploadImageAsync()`，回傳 201
- [x] 7.2 在 `ProductController.cs` 新增 `DELETE /api/products/{id}/images/{imageId}` 端點，呼叫 `ProductService.RemoveImageAsync()`，回傳 204
- [x] 7.3 在 `ProductController.cs` 新增 `PATCH /api/products/{id}/images/{imageId}/set-cover` 端點，呼叫 `ProductService.SetCoverImageAsync()`，回傳 200
- [x] 7.4 確認 `ProductImageNotFoundException` 在 Controller 或全域例外處理中對應到 HTTP 404
