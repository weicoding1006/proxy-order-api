## Why

`Order`、`OrderItem` 實體與 `OrderDbContext` 已完整定義，Migration 也已建立。目前缺少應用層與 API 層：使用者無法透過 API 下單或查詢訂單。本次變更補齊 Repository、Service、DTO 與 Controller，讓系統具備完整的訂單 CRUD 能力。

## What Changes

- 新增 `IOrderRepository` 介面（Application 層）
- 新增 `OrderRepository` 實作（Infrastructure 層）
- 新增訂單相關 DTO：`CreateOrderRequest`、`OrderResponse`、`OrderItemResponse`
- 新增 `OrderService`，包含建立訂單（含庫存驗證與 UnitPrice 快照）及查詢邏輯
- 新增 `OrdersController`，提供：
  - `POST /api/orders`（建立訂單，需 JWT 認證）
  - `GET /api/orders`（查詢當前使用者的所有訂單，需 JWT 認證）
  - `GET /api/orders/{id}`（查詢單筆訂單，限當前使用者，需 JWT 認證）

## Capabilities

### New Capabilities

- `order-repository`: `IOrderRepository` 介面與 `OrderRepository` 實作，封裝 Order / OrderItem 資料存取
- `order-service`: 訂單業務邏輯，含建立訂單時的庫存檢查、UnitPrice 快照、TotalAmount 計算
- `order-dto`: `CreateOrderRequest`、`OrderItemRequest`、`OrderResponse`、`OrderItemResponse`
- `order-controller`: `OrdersController` REST API，所有端點使用 `[Authorize]`，並從 JWT claims 取得 UserId

### Modified Capabilities

（無需修改現有 spec，資料模型與 DbContext 已在前次變更完成）

## Impact

- **Application 層**：新增 `Interfaces/IOrderRepository.cs`、`Services/OrderService.cs`、`DTOs/Order/` 目錄
- **Infrastructure 層**：新增 `Repositories/OrderRepository.cs`，注入 `OrderDbContext`
- **Api 層**：新增 `Controllers/OrdersController.cs`，注入 `OrderService`
- **DI 註冊**：`Program.cs` 需新增 `OrderRepository` 與 `OrderService` 的 DI 綁定
- **相依性**：`OrderService` 需注入 `IProductRepository` 以驗證商品存在與庫存數量
