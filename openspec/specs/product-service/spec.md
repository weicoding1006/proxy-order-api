## ADDED Requirements

### Requirement: 建立產品業務邏輯
ProductService SHALL 提供 `create(dto: CreateProductDto): Promise<ProductResponseDto>` 方法，執行業務驗證後呼叫 Repository 儲存，並回傳 `ProductResponseDto`。

#### Scenario: 成功建立產品
- **WHEN** 傳入有效的 `CreateProductDto`（`name` 非空、`price` > 0）
- **THEN** Service 呼叫 `IProductRepository.save()`，並回傳包含新 ID 的 `ProductResponseDto`

#### Scenario: 價格為負數
- **WHEN** 傳入 `CreateProductDto`，其中 `price` 為 -1
- **THEN** Service 拋出業務例外 `InvalidProductDataException`，不呼叫 Repository

---

### Requirement: 查詢所有產品業務邏輯
ProductService SHALL 提供 `findAll(): Promise<ProductResponseDto[]>` 方法，呼叫 Repository 取得所有產品並回傳。

#### Scenario: 有產品時回傳列表
- **WHEN** 資料庫中存有 3 筆產品記錄
- **THEN** Service 回傳包含 3 個 `ProductResponseDto` 的陣列

#### Scenario: 無產品時回傳空陣列
- **WHEN** 資料庫中無任何產品記錄
- **THEN** Service 回傳空陣列 `[]`，不拋出例外

---

### Requirement: 查詢單一產品業務邏輯
ProductService SHALL 提供 `findOne(id: number): Promise<ProductResponseDto>` 方法，若產品不存在 MUST 拋出 `ProductNotFoundException`。

#### Scenario: 產品存在
- **WHEN** 傳入存在的產品 ID
- **THEN** Service 回傳對應的 `ProductResponseDto`

#### Scenario: 產品不存在
- **WHEN** 傳入不存在的產品 ID
- **THEN** Service 拋出 `ProductNotFoundException`，不回傳資料

---

### Requirement: 更新產品業務邏輯
ProductService SHALL 提供 `update(id: number, dto: UpdateProductDto): Promise<ProductResponseDto>` 方法，先確認產品存在後再執行更新。

#### Scenario: 成功更新
- **WHEN** 傳入存在的產品 ID 與有效的 `UpdateProductDto`
- **THEN** Service 呼叫 Repository 更新，並回傳更新後的 `ProductResponseDto`

#### Scenario: 更新不存在的產品
- **WHEN** 傳入不存在的產品 ID
- **THEN** Service 拋出 `ProductNotFoundException`，不執行任何更新操作

---

### Requirement: 刪除產品業務邏輯
ProductService SHALL 提供 `remove(id: number): Promise<void>` 方法，先確認產品存在後再執行刪除。

#### Scenario: 成功刪除
- **WHEN** 傳入存在的產品 ID
- **THEN** Service 呼叫 Repository 刪除，完成後不回傳資料

#### Scenario: 刪除不存在的產品
- **WHEN** 傳入不存在的產品 ID
- **THEN** Service 拋出 `ProductNotFoundException`，不執行刪除

---

### Requirement: Service 不直接存取資料庫
ProductService MUST 透過 `IProductRepository` Interface 存取資料，不得直接使用任何 ORM 或資料庫 API。

#### Scenario: 透過介面存取資料
- **WHEN** Service 任何方法需要讀寫資料
- **THEN** Service 只呼叫 `IProductRepository` 上定義的方法，不包含任何 SQL 或 ORM 查詢語法
