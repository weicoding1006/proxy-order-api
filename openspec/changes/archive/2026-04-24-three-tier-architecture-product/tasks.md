## 1. 基礎設定與 Entity

- [x] 1.1 建立 `src/product/` 模組目錄結構（controller / service / repository / dto / entities）
- [x] 1.2 定義 `Product` Entity，包含 `id`、`name`、`price`、`description`、`createdAt`、`updatedAt` 欄位
- [x] 1.3 確認 EF Core 設定，將 `Product` Entity 加入 `OrderDbContext`（已完成）

## 2. DTO 定義

- [x] 2.1 建立 `CreateProductDto`，加入欄位驗證（`name` 必填、`price` > 0）
- [x] 2.2 建立 `UpdateProductDto`（所有欄位 Partial）
- [x] 2.3 建立 `ProductResponseDto`，明確定義回應欄位，排除內部欄位

## 3. Repository 層

- [x] 3.1 定義 `IProductRepository` Interface，包含 `findAll`、`findById`、`save`、`update`、`delete` 方法簽章
- [x] 3.2 實作 `ProductRepository`，實作 `IProductRepository` 所有方法
- [x] 3.3 驗證 `findById` 在找不到資料時回傳 `null`（不拋出例外）
- [ ] 3.4 撰寫 `ProductRepository` 整合測試（需連接測試資料庫）

## 4. Service 層

- [x] 4.1 建立 `ProductService`，透過建構子注入 `IProductRepository`
- [x] 4.2 實作 `create(dto)` 方法，含業務驗證（價格不能 ≤ 0）
- [x] 4.3 實作 `findAll()` 方法
- [x] 4.4 實作 `findOne(id)` 方法，若找不到則拋出 `ProductNotFoundException`
- [x] 4.5 實作 `update(id, dto)` 方法，先確認產品存在
- [x] 4.6 實作 `remove(id)` 方法，先確認產品存在
- [x] 4.7 建立自訂例外類別：`ProductNotFoundException`、`InvalidProductDataException`
- [ ] 4.8 撰寫 `ProductService` 單元測試（mock `IProductRepository`）

## 5. Controller 層

- [x] 5.1 建立 `ProductController`，透過建構子注入 `ProductService`
- [x] 5.2 實作 `POST /products` 端點，回傳 HTTP 201
- [x] 5.3 實作 `GET /products` 端點，回傳 HTTP 200
- [x] 5.4 實作 `GET /products/:id` 端點，回傳 HTTP 200 或 404
- [x] 5.5 實作 `PATCH /products/:id` 端點，回傳 HTTP 200 或 404
- [x] 5.6 實作 `DELETE /products/:id` 端點，回傳 HTTP 204 或 404
- [x] 5.7 確認 Controller 內無任何業務邏輯或 ORM 呼叫
- [ ] 5.8 撰寫 `ProductController` 單元測試（mock `ProductService`）

## 6. 整合與驗收

- [ ] 6.1 撰寫端對端整合測試（E2E）：建立 → 查詢 → 更新 → 刪除完整流程
- [x] 6.2 驗證錯誤回應格式符合 API 規範（404 含錯誤訊息、400 含欄位錯誤）
- [x] 6.3 確認架構符合三層分離原則（使用 lint 或 dependency-cruiser 驗證跨層依賴）
