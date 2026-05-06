## Why

目前系統僅支援直接建立訂單，使用者無法先將商品加入購物車後再統一結帳，缺乏購物車緩衝層導致使用體驗不佳且無法支援多品項一次下單的流程。

## What Changes

- 新增 `Cart` 實體：儲存每位使用者的購物車（一人一車）
- 新增 `CartItem` 實體：購物車內的商品條目（含數量與當下商品快照參照）
- `ApplicationUser` 增加導覽屬性 `Cart`（一對一關聯）
- 新增購物車 CRUD API：加入商品、更新數量、移除商品、清空購物車、查詢購物車
- 新增「將購物車轉換為訂單」API：從 Cart 建立 Order + OrderItems，並清空購物車
- 新增 EF Core Migration 以建立 `Carts` / `CartItems` 資料表

## Capabilities

### New Capabilities
- `cart-management`: 管理使用者購物車（新增、查詢、更新、刪除購物車商品）
- `cart-checkout`: 將購物車轉換為正式訂單並清空購物車

### Modified Capabilities
- `order-management`: 增加「從購物車建立訂單」的入口，原有直接建立訂單流程保持不變

## Impact

- **新增 Domain 實體**：`Cart`、`CartItem`（位於 `OrderSystem.Domain/Entities/`）
- **修改 `ApplicationUser`**：加入 `Cart` 導覽屬性
- **資料庫**：新增 `Carts`（UserId UNIQUE）與 `CartItems`（CartId + ProductId UNIQUE）資料表
- **新增 Repository**：`ICartRepository` / `CartRepository`
- **新增 Application 層**：CartService、Cart DTOs、Cart Commands/Queries
- **新增 API 端點**：`/api/cart`（GET、POST、PUT、DELETE）與 `/api/cart/checkout`（POST）
- **現有 OrderService**：需支援接受來自 CartCheckout 的呼叫路徑
