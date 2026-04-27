## Why

目前系統缺乏清晰的關注點分離，導致業務邏輯、資料存取與介面層混雜，造成維護困難與測試成本高。透過引入三層式架構（Presentation → Service → Repository），以 Product 模組為範例，建立可複用的分層標準。

## What Changes

- 新增 `ProductController`：處理 HTTP 請求/回應，不含業務邏輯
- 新增 `ProductService`：封裝所有業務規則（建立、查詢、更新、刪除）
- 新增 `ProductRepository`：負責所有資料庫存取操作
- 新增 `Product` 實體/模型定義
- 新增 DTO（Data Transfer Object）：`CreateProductDto`、`UpdateProductDto`、`ProductResponseDto`
- 各層僅透過介面（Interface）相依，降低耦合

## Capabilities

### New Capabilities

- `product-controller`: HTTP 端點層，處理路由、請求驗證與回應格式化
- `product-service`: 業務邏輯層，處理產品的 CRUD 業務規則與驗證
- `product-repository`: 資料存取層，封裝所有 ORM/資料庫查詢
- `product-dto`: 資料傳輸物件定義，確保層間資料結構清晰

### Modified Capabilities

<!-- 目前為新專案範例，無既有 spec 需修改 -->

## Impact

- **新增檔案**: Controller、Service、Repository、DTO、Entity 各一組
- **依賴**: 需要 ORM 套件（如 TypeORM / Prisma）與框架（如 NestJS / Express）
- **測試**: 各層可獨立進行單元測試，Service 層可 mock Repository
- **擴展性**: 此架構模式可複製至其他模組（Order、User 等）
