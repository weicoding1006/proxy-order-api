## ADDED Requirements

### Requirement: Repository Interface 定義
系統 SHALL 定義 `IProductRepository` Interface，包含以下方法：
- `findAll(): Promise<Product[]>`
- `findById(id: number): Promise<Product | null>`
- `save(product: Partial<Product>): Promise<Product>`
- `update(id: number, data: Partial<Product>): Promise<Product>`
- `delete(id: number): Promise<void>`

#### Scenario: Interface 作為依賴注入契約
- **WHEN** `ProductService` 被初始化
- **THEN** Service 透過建構子注入 `IProductRepository`，不直接依賴具體實作類別

---

### Requirement: 查詢所有產品
`ProductRepository.findAll()` SHALL 從資料庫取得所有未被軟刪除的產品記錄。

#### Scenario: 成功查詢
- **WHEN** 呼叫 `findAll()`
- **THEN** 回傳所有 `Product` 記錄的陣列，若無記錄則回傳空陣列

---

### Requirement: 依 ID 查詢產品
`ProductRepository.findById(id)` SHALL 回傳指定 ID 的產品，若不存在 MUST 回傳 `null`（不拋出例外）。

#### Scenario: 產品存在
- **WHEN** 傳入存在的 ID
- **THEN** 回傳對應的 `Product` 物件

#### Scenario: 產品不存在
- **WHEN** 傳入不存在的 ID
- **THEN** 回傳 `null`，不拋出任何例外

---

### Requirement: 儲存新產品
`ProductRepository.save(product)` SHALL 將新產品持久化至資料庫，並回傳包含自動產生 ID 的完整 `Product` 物件。

#### Scenario: 成功儲存
- **WHEN** 傳入有效的產品資料
- **THEN** 資料庫新增一筆記錄，回傳包含 `id`、`createdAt` 的完整 `Product`

---

### Requirement: 更新產品
`ProductRepository.update(id, data)` SHALL 更新指定 ID 產品的欄位，並回傳更新後的完整 `Product` 物件。

#### Scenario: 成功更新
- **WHEN** 傳入存在的 ID 與部分更新資料
- **THEN** 資料庫對應記錄被更新，回傳更新後的 `Product`

---

### Requirement: 刪除產品
`ProductRepository.delete(id)` SHALL 從資料庫移除指定 ID 的產品記錄。

#### Scenario: 成功刪除
- **WHEN** 傳入存在的 ID
- **THEN** 資料庫對應記錄被刪除，方法完成後不回傳資料

---

### Requirement: Repository 不含業務邏輯
`ProductRepository` MUST 只執行資料庫 CRUD 操作，不含任何業務驗證規則。

#### Scenario: 純資料操作
- **WHEN** 呼叫任何 Repository 方法
- **THEN** Repository 只執行 ORM 查詢，不做業務判斷（如「價格不能為負」屬於 Service 的責任）
