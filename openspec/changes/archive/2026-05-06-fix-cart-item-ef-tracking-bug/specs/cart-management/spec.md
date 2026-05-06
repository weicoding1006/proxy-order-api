## ADDED Requirements

### Requirement: CartRepository 明確插入 CartItem
`ICartRepository` SHALL 提供 `Task<CartItem> AddCartItemAsync(CartItem item)` 方法，內部使用 `context.CartItems.Add(item)` 明確將 CartItem 設定為 `EntityState.Added`，再呼叫 `SaveChangesAsync()`。`CartService` 加入新 CartItem 時 SHALL 呼叫此方法，禁止直接對 `cart.CartItems` collection 做 `Add` 後再呼叫 `SaveChangesAsync()`，以確保 EF Core 永遠發出 INSERT 而非 UPDATE。

#### Scenario: 新商品加入空購物車時發出 INSERT
- **WHEN** 使用者 `POST /api/cart/items`，購物車為空，商品尚未存在
- **THEN** 資料庫收到 `INSERT INTO "CartItems"` 指令，不得出現 `UPDATE "CartItems"`，回傳 200 OK 含更新後的購物車

#### Scenario: 新商品加入已有商品的購物車時發出 INSERT
- **WHEN** 購物車已含其他商品，使用者加入一個尚未在購物車的商品
- **THEN** 資料庫收到 `INSERT INTO "CartItems"` 指令，回傳 200 OK，原有商品不受影響

### Requirement: 使用者刪除時 Cascade 刪除 Cart 與 CartItems
`OrderDbContext` 的 Cart entity 設定 SHALL 在 User → Cart 的外鍵關係上設定 `OnDelete(DeleteBehavior.Cascade)`，確保使用者帳號刪除時 Cart 與所有 CartItems 自動跟著刪除，不得出現 FK 約束錯誤。

#### Scenario: 使用者刪除後 Cart 自動清除
- **WHEN** `ApplicationUser` 從資料庫刪除
- **THEN** 對應的 `Cart` 記錄與所有 `CartItems` 同時被刪除，不需應用層額外操作

## MODIFIED Requirements

### Requirement: 加入商品至購物車（Upsert 語意）
`CartService` SHALL 提供 `AddItemAsync(string userId, Guid productId, int quantity)` 方法：若商品已在購物車則累加數量，否則透過 `ICartRepository.AddCartItemAsync` 新增 CartItem。商品必須存在且 `IsActive = true`。建立新 CartItem 時不得手動指定 `Id`，由 EF Core 透過 `ValueGeneratedOnAdd` 自動生成。

#### Scenario: 新商品加入購物車
- **WHEN** 使用者 `POST /api/cart/items`，商品尚未在購物車
- **THEN** 新增 CartItem，回傳 200 OK 含更新後的購物車

#### Scenario: 既有商品累加數量
- **WHEN** 使用者 `POST /api/cart/items`，商品已在購物車（Quantity=2），新增 Quantity=3
- **THEN** CartItem.Quantity 更新為 5，回傳 200 OK

#### Scenario: 商品不存在或已下架
- **WHEN** 使用者加入不存在或 `IsActive = false` 的商品
- **THEN** 回傳 400 Bad Request 含錯誤訊息
