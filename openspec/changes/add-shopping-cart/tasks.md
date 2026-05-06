## 1. Domain 實體

- [x] 1.1 新增 `Cart` 實體（`OrderSystem.Domain/Entities/Cart.cs`），包含 Id、UserId、CreatedAt、UpdatedAt、CartItems 導覽屬性
- [x] 1.2 新增 `CartItem` 實體（`OrderSystem.Domain/Entities/CartItem.cs`），包含 Id、CartId、ProductId、Quantity 及對應導覽屬性
- [x] 1.3 在 `ApplicationUser` 加入 `Cart? Cart` nullable 導覽屬性

## 2. EF Core 設定與 Migration

- [x] 2.1 在 `OrderDbContext.OnModelCreating` 設定 `Cart`：UserId UNIQUE INDEX、與 ApplicationUser 一對一關聯
- [x] 2.2 在 `OrderDbContext.OnModelCreating` 設定 `CartItem`：(CartId, ProductId) 複合 UNIQUE INDEX、Quantity CHECK ≥ 1、Cart CASCADE DELETE
- [x] 2.3 執行 `dotnet ef migrations add AddShoppingCart --project OrderSystem.Infrastructure --startup-project OrderSystem.Api`
- [x] 2.4 套用 Migration（`dotnet ef database update`）並確認 `Carts` / `CartItems` 資料表已建立

## 3. Repository

- [x] 3.1 新增 `ICartRepository` 介面（`OrderSystem.Application` 或 Domain 層），定義 `GetByUserIdAsync`、`AddAsync`、`SaveChangesAsync`
- [x] 3.2 實作 `CartRepository`（`OrderSystem.Infrastructure`），查詢時 Include CartItems 與 Product

## 4. Application — DTOs

- [x] 4.1 新增 `CartItemResponse` DTO（ProductId、ProductName、CurrentPrice、Quantity、Subtotal）
- [x] 4.2 新增 `CartResponse` DTO（CartId、Items、TotalAmount）
- [x] 4.3 新增 `AddCartItemRequest` DTO（ProductId、Quantity）
- [x] 4.4 新增 `UpdateCartItemRequest` DTO（Quantity）

## 5. Application — CartService

- [x] 5.1 新增 `ICartService` 介面，定義 GetOrCreateCartAsync、AddItemAsync、UpdateItemQuantityAsync、RemoveItemAsync、ClearCartAsync、CheckoutAsync
- [x] 5.2 實作 `CartService.GetOrCreateCartAsync`：查詢購物車，不存在時建立並儲存
- [x] 5.3 實作 `CartService.AddItemAsync`：驗證商品存在且 IsActive，Upsert CartItem（已存在則累加數量）
- [x] 5.4 實作 `CartService.UpdateItemQuantityAsync`：驗證 CartItem 屬於當前使用者，quantity=0 時刪除，否則更新
- [x] 5.5 實作 `CartService.RemoveItemAsync`：驗證擁有權後刪除 CartItem
- [x] 5.6 實作 `CartService.ClearCartAsync`：刪除購物車內所有 CartItems
- [x] 5.7 實作 `CartService.CheckoutAsync`：購物車為空拋出例外，否則組裝 CreateOrderRequest 並呼叫 OrderService.CreateAsync，成功後清空購物車

## 6. API 端點

- [x] 6.1 新增 `CartController`，加上 `[Authorize]` 與路由 `/api/cart`
- [x] 6.2 實作 `GET /api/cart`：呼叫 GetOrCreateCartAsync，回傳 200 + CartResponse
- [x] 6.3 實作 `POST /api/cart/items`：呼叫 AddItemAsync，回傳 200 + CartResponse
- [x] 6.4 實作 `PUT /api/cart/items/{cartItemId}`：呼叫 UpdateItemQuantityAsync，回傳 200 + CartResponse 或 404
- [x] 6.5 實作 `DELETE /api/cart/items/{cartItemId}`：呼叫 RemoveItemAsync，回傳 204 或 404
- [x] 6.6 實作 `DELETE /api/cart`：呼叫 ClearCartAsync，回傳 204
- [x] 6.7 實作 `POST /api/cart/checkout`：呼叫 CheckoutAsync，回傳 201 + OrderResponse 或 400

## 7. DI 註冊與驗證

- [x] 7.1 在 `Program.cs` 註冊 `ICartRepository` / `CartRepository` 與 `ICartService` / `CartService`
- [x] 7.2 啟動 API 確認無例外，Swagger / Scalar UI 顯示購物車端點
- [ ] 7.3 手動測試：加入商品、更新數量、移除商品、結帳建立訂單、空購物車結帳回傳 400
