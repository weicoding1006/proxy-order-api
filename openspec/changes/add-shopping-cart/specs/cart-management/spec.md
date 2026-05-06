## ADDED Requirements

### Requirement: Cart 與 CartItem Domain 實體定義
系統 SHALL 新增 `Cart` 與 `CartItem` 兩個 Domain 實體，放置於 `OrderSystem.Domain/Entities/`。

`Cart` 屬性：
- `Id (Guid)` — 主鍵
- `UserId (string)` — 外鍵，指向 `ApplicationUser`，UNIQUE
- `User (ApplicationUser)` — 導覽屬性
- `CartItems (ICollection<CartItem>)` — 一對多導覽屬性
- `CreatedAt (DateTime)` — 建立時間（UTC）
- `UpdatedAt (DateTime)` — 最後更新時間（UTC）

`CartItem` 屬性：
- `Id (Guid)` — 主鍵
- `CartId (Guid)` — 外鍵，指向 `Cart`
- `Cart (Cart)` — 導覽屬性
- `ProductId (Guid)` — 外鍵，指向 `Product`
- `Product (Product)` — 導覽屬性
- `Quantity (int)` — 數量，須 ≥ 1

#### Scenario: Cart 實體屬性存在
- **WHEN** 建立 `Cart` 實體並儲存至資料庫
- **THEN** 資料庫中 `Carts` 資料表須包含 Id、UserId、CreatedAt、UpdatedAt 欄位，且 UserId 有 UNIQUE 約束

#### Scenario: CartItem 實體屬性存在
- **WHEN** 建立 `CartItem` 實體並儲存至資料庫
- **THEN** 資料庫中 `CartItems` 資料表須包含 Id、CartId、ProductId、Quantity 欄位，且 (CartId, ProductId) 有複合 UNIQUE 約束

### Requirement: ApplicationUser 擴充 Cart 導覽屬性
`ApplicationUser` SHALL 新增 `Cart? Cart` 導覽屬性（nullable，一對一）。

#### Scenario: 透過 User 查詢購物車
- **WHEN** 使用 `dbContext.Users.Include(u => u.Cart).ThenInclude(c => c.CartItems)` 查詢
- **THEN** 回傳的 `ApplicationUser` 物件包含已填充的 `Cart` 與 `CartItems`

### Requirement: EF Core 設定購物車資料表
`OrderDbContext.OnModelCreating` SHALL 設定：
- `Cart.UserId` 為 UNIQUE INDEX
- `CartItem` 以 (CartId, ProductId) 為複合 UNIQUE INDEX
- `CartItem.Quantity` CHECK 約束須 ≥ 1
- `Cart` 刪除時 CASCADE DELETE 所有 `CartItems`

#### Scenario: Migration 產生正確資料表
- **WHEN** 執行 Migration 並套用
- **THEN** 資料庫新增 `Carts` 與 `CartItems` 資料表，含正確外鍵、UNIQUE INDEX 與 CASCADE DELETE 設定

### Requirement: 查詢購物車（自動建立）
`CartService` SHALL 提供 `GetOrCreateCartAsync(string userId)` 方法：若使用者購物車不存在則自動建立並回傳，存在則直接回傳（含 CartItems 與 Product 資訊）。

#### Scenario: 首次查詢自動建立
- **WHEN** 使用者首次呼叫 `GET /api/cart`（購物車不存在）
- **THEN** 系統自動建立空購物車並回傳 200 OK，`items` 為空陣列

#### Scenario: 查詢已有購物車
- **WHEN** 使用者購物車已存在且含商品
- **THEN** 回傳 200 OK，`items` 包含各 CartItem 及對應商品資訊（名稱、當前價格）

### Requirement: 加入商品至購物車（Upsert 語意）
`CartService` SHALL 提供 `AddItemAsync(string userId, Guid productId, int quantity)` 方法：若商品已在購物車則累加數量，否則新增 CartItem。商品必須存在且 `IsActive = true`。

#### Scenario: 新商品加入購物車
- **WHEN** 使用者 `POST /api/cart/items`，商品尚未在購物車
- **THEN** 新增 CartItem，回傳 200 OK 含更新後的購物車

#### Scenario: 既有商品累加數量
- **WHEN** 使用者 `POST /api/cart/items`，商品已在購物車（Quantity=2），新增 Quantity=3
- **THEN** CartItem.Quantity 更新為 5，回傳 200 OK

#### Scenario: 商品不存在或已下架
- **WHEN** 使用者加入不存在或 `IsActive = false` 的商品
- **THEN** 回傳 400 Bad Request 含錯誤訊息

### Requirement: 更新購物車商品數量
`CartService` SHALL 提供 `UpdateItemQuantityAsync(string userId, Guid cartItemId, int quantity)` 方法：更新指定 CartItem 的數量，數量須 ≥ 1。若 quantity = 0，視同移除該 CartItem。

#### Scenario: 正常更新數量
- **WHEN** 使用者 `PUT /api/cart/items/{cartItemId}`，quantity = 5
- **THEN** CartItem.Quantity 更新為 5，回傳 200 OK

#### Scenario: 數量設為 0 移除商品
- **WHEN** 使用者 `PUT /api/cart/items/{cartItemId}`，quantity = 0
- **THEN** 該 CartItem 從資料庫刪除，回傳 200 OK

#### Scenario: CartItem 不屬於當前使用者
- **WHEN** 使用者嘗試更新不屬於自己購物車的 CartItem
- **THEN** 回傳 404 Not Found

### Requirement: 移除購物車單項商品
`CartService` SHALL 提供 `RemoveItemAsync(string userId, Guid cartItemId)` 方法，從購物車移除指定 CartItem。

#### Scenario: 成功移除
- **WHEN** 使用者 `DELETE /api/cart/items/{cartItemId}`，CartItem 屬於自己購物車
- **THEN** CartItem 從資料庫刪除，回傳 204 No Content

#### Scenario: CartItem 不存在
- **WHEN** CartItemId 不存在或不屬於當前使用者
- **THEN** 回傳 404 Not Found

### Requirement: 清空購物車
`CartService` SHALL 提供 `ClearCartAsync(string userId)` 方法，刪除購物車內所有 CartItems（保留 Cart 記錄）。

#### Scenario: 成功清空
- **WHEN** 使用者 `DELETE /api/cart`
- **THEN** 所有 CartItems 刪除，Cart 保留，回傳 204 No Content
