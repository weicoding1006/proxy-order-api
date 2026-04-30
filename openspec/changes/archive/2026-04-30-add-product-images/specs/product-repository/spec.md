## MODIFIED Requirements

### Requirement: Repository Interface 定義
系統 SHALL 定義 `IProductRepository` Interface，包含以下方法：
- `findAll(): Task<List<Product>>`（含 Images 集合，依 SortOrder 升冪）
- `findById(id: Guid): Task<Product?>`（含 Images 集合，依 SortOrder 升冪）
- `save(product: Product): Task<Product>`
- `update(id: Guid, data: UpdateProductDto): Task<Product>`
- `delete(id: Guid): Task`
- `addImage(image: ProductImage): Task<ProductImage>`
- `findImageById(imageId: Guid): Task<ProductImage?>`
- `removeImage(image: ProductImage): Task`
- `saveChangesAsync(): Task`

#### Scenario: Interface 作為依賴注入契約
- **WHEN** `ProductService` 被初始化
- **THEN** Service 透過建構子注入 `IProductRepository`，不直接依賴具體實作類別

---

### Requirement: 查詢產品時一併載入圖片
`ProductRepository.findAll()` 與 `findById()` SHALL 使用 EF Core eager loading（`.Include(p => p.Images).ThenInclude(...)`）一次載入產品與其所有圖片，不使用 lazy loading。

#### Scenario: 查詢單一產品含圖片
- **WHEN** 呼叫 `findById(id)`，且該產品有圖片
- **THEN** 回傳的 `Product.Images` 集合已載入，不需額外查詢

#### Scenario: 避免 N+1 查詢
- **WHEN** 呼叫 `findAll()` 取得 10 個產品
- **THEN** 只發出 1 或 2 條 SQL（JOIN 查詢），不發出 10 條額外的圖片查詢

## ADDED Requirements

### Requirement: 新增圖片記錄
`ProductRepository.addImage(image)` SHALL 將 `ProductImage` 新增至資料庫，並回傳包含自動產生 ID 的完整 `ProductImage`。

#### Scenario: 成功新增
- **WHEN** 傳入有效的 `ProductImage`（含 ProductId、ImageUrl、IsCover、SortOrder）
- **THEN** 資料庫新增一筆 `ProductImages` 記錄，回傳含 `Id` 的完整物件

---

### Requirement: 依 ID 查詢圖片
`ProductRepository.findImageById(imageId)` SHALL 回傳指定 ID 的 `ProductImage`，若不存在回傳 `null`。

#### Scenario: 圖片存在
- **WHEN** 傳入存在的 `imageId`
- **THEN** 回傳對應的 `ProductImage` 物件（含 `ProductId`）

#### Scenario: 圖片不存在
- **WHEN** 傳入不存在的 `imageId`
- **THEN** 回傳 `null`，不拋出任何例外

---

### Requirement: 刪除圖片記錄
`ProductRepository.removeImage(image)` SHALL 從資料庫移除指定的 `ProductImage` 記錄。

#### Scenario: 成功刪除
- **WHEN** 傳入存在的 `ProductImage` 物件
- **THEN** 資料庫對應記錄被移除
