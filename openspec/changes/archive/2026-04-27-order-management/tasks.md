## 1. DTOs

- [x] 1.1 建立 `OrderSystem.Application/DTOs/Order/OrderItemRequest.cs`（ProductId: Guid, Quantity: int）
- [x] 1.2 建立 `OrderSystem.Application/DTOs/Order/CreateOrderRequest.cs`（Items: List<OrderItemRequest>，含 validation attributes）
- [x] 1.3 建立 `OrderSystem.Application/DTOs/Order/OrderItemResponse.cs`（Id, ProductId, Quantity, UnitPrice）
- [x] 1.4 建立 `OrderSystem.Application/DTOs/Order/OrderResponse.cs`（Id, UserId, TotalAmount, Status, CreatedAt, Items），含靜態 `FromEntity` 方法

## 2. Domain Exception

- [x] 2.1 建立 `OrderSystem.Domain/Exceptions/InsufficientStockException.cs`，訊息含商品名稱與可用庫存數量

## 3. Repository

- [x] 3.1 建立 `OrderSystem.Application/Interfaces/IOrderRepository.cs`，定義 `CreateAsync`、`FindByUserIdAsync`、`FindByIdAsync`
- [x] 3.2 建立 `OrderSystem.Infrastructure/Repositories/OrderRepository.cs`，實作 `IOrderRepository`，使用 primary constructor 注入 `OrderDbContext`，`FindByUserIdAsync` 與 `FindByIdAsync` 需 `.Include(o => o.OrderItems)`

## 4. Service

- [x] 4.1 建立 `OrderSystem.Application/Services/OrderService.cs`，注入 `IOrderRepository` 與 `IProductRepository`
- [x] 4.2 實作 `CreateAsync(CreateOrderRequest dto, string userId)`：驗證商品存在、IsActive、庫存充足，快照 UnitPrice，計算 TotalAmount，呼叫 repository 儲存
- [x] 4.3 實作 `FindByUserIdAsync(string userId)`：回傳 `List<OrderResponse>`
- [x] 4.4 實作 `FindOneAsync(Guid id, string userId)`：回傳 `OrderResponse?`，UserId 不符時回傳 null

## 5. DI 註冊

- [x] 5.1 在 `Program.cs` 新增 `builder.Services.AddScoped<IOrderRepository, OrderRepository>()`
- [x] 5.2 在 `Program.cs` 新增 `builder.Services.AddScoped<OrderService>()`

## 6. Controller

- [x] 6.1 建立 `OrderSystem.Api/Controllers/OrdersController.cs`，加上 `[Authorize]` 與 `[Route("api/orders")]`
- [x] 6.2 實作 `POST /api/orders`：從 JWT claims 取 UserId，呼叫 `OrderService.CreateAsync`，捕捉 `ProductNotFoundException`（→ 404）與 `InsufficientStockException`（→ 400），成功回傳 `201 Created`
- [x] 6.3 實作 `GET /api/orders`：呼叫 `OrderService.FindByUserIdAsync`，回傳 `200 OK`
- [x] 6.4 實作 `GET /api/orders/{id:guid}`：呼叫 `OrderService.FindOneAsync`，null 時回傳 `404 Not Found`

## 7. 驗證

- [x] 7.1 執行 `dotnet build` 確認無編譯錯誤
- [ ] 7.2 透過 Scalar / Swagger 以有效 JWT 測試 `POST /api/orders`（正常下單）
- [ ] 7.3 測試庫存不足情境（預期 400）
- [ ] 7.4 測試 `GET /api/orders` 只回傳自己的訂單
- [ ] 7.5 測試 `GET /api/orders/{id}` 存取他人訂單回傳 404
