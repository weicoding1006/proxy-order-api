## MODIFIED Requirements

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
