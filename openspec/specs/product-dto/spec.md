## ADDED Requirements

### Requirement: CreateProductDto 定義
系統 SHALL 定義 `CreateProductDto`，包含以下欄位：
- `name: string`（必填，非空字串）
- `price: number`（必填，大於 0）
- `description?: string`（選填）

#### Scenario: 有效的 CreateProductDto
- **WHEN** 傳入 `{ name: "Widget", price: 9.99 }`
- **THEN** DTO 驗證通過，可傳遞給 Service

#### Scenario: 缺少必填欄位的 CreateProductDto
- **WHEN** 傳入 `{ price: 9.99 }`（缺少 `name`）
- **THEN** DTO 驗證失敗，回傳欄位驗證錯誤

---

### Requirement: UpdateProductDto 定義
系統 SHALL 定義 `UpdateProductDto`，所有欄位均為選填（Partial），至少需包含一個欄位：
- `name?: string`
- `price?: number`
- `description?: string`

#### Scenario: 部分更新
- **WHEN** 傳入 `{ price: 19.99 }`（只更新價格）
- **THEN** DTO 驗證通過，Service 只更新 `price` 欄位，其他欄位保持不變

---

### Requirement: ProductResponseDto 定義
系統 SHALL 定義 `ProductResponseDto`，包含以下欄位（映射自 `Product` Entity）：
- `id: Guid`
- `name: string`
- `price: decimal`
- `description: string | null`
- `stock: int`
- `isActive: bool`
- `images: ProductImageDto[]`（圖片陣列，無圖片時為空陣列，依 `sortOrder` 升冪排列）
- `createdAt: DateTime`
- `updatedAt: DateTime`

#### Scenario: 回應格式標準化
- **WHEN** Service 回傳產品資料給 Controller
- **THEN** Controller 收到的是 `ProductResponseDto`，不包含資料庫內部欄位（如 `deletedAt`、ORM 元資料）

#### Scenario: 無圖片時回傳空陣列
- **WHEN** 產品尚未上傳任何圖片
- **THEN** `ProductResponseDto.images` 為 `[]`，不為 `null`

#### Scenario: 有圖片時依排序回傳
- **WHEN** 產品有多張圖片
- **THEN** `images` 陣列依 `sortOrder` 升冪排列，封面圖可透過 `isCover: true` 識別

---

### Requirement: DTO 隔離 Entity 細節
所有 DTO MUST 與 `Product` Entity 分離，Entity 欄位變更不應直接影響 API 回應格式。

#### Scenario: Entity 新增內部欄位
- **WHEN** `Product` Entity 新增 `internalAuditLog` 欄位
- **THEN** `ProductResponseDto` 不自動包含此欄位，需明確加入 DTO 定義
