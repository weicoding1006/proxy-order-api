## 1. Repository 介面與實作

- [x] 1.1 在 `ICartRepository` 新增 `Task<CartItem> AddCartItemAsync(CartItem item)` 方法簽章
- [x] 1.2 在 `CartRepository` 實作 `AddCartItemAsync`：呼叫 `context.CartItems.Add(item)` 後 `await context.SaveChangesAsync()`，回傳 item

## 2. Service 修正

- [x] 2.1 在 `CartService.AddItemAsync` 的「新增 CartItem」分支，改為呼叫 `cartRepository.AddCartItemAsync(cartItem)` 取代 `cart.CartItems.Add(cartItem)` + `SaveChangesAsync`
- [x] 2.2 建立 `CartItem` 時移除手動 `Id = Guid.NewGuid()` 指定，改為 `new CartItem { CartId = cart.Id, ProductId = productId, Quantity = quantity }`

## 3. Cart Cascade Delete 補強

- [x] 3.1 在 `OrderDbContext` 的 Cart entity 設定補上 `.OnDelete(DeleteBehavior.Cascade)`（資料庫行為早已正確，此變更讓 fluent config 明確表達意圖）
- [x] 3.2 確認不需要新 Migration（原始 `AddShoppingCart` Migration 已是 `ReferentialAction.Cascade`）

## 4. 驗證

- [ ] 4.1 啟動 API，`POST /api/cart/items` 加入全新商品（購物車為空），確認回傳 200 OK 且不出現 `DbUpdateConcurrencyException`
- [ ] 4.2 確認已有商品的購物車累加數量（existing path）依然正常
- [ ] 4.3 確認 `POST /api/cart/items` 加入第二個不同商品時同樣回傳 200 OK
<!-- 驗證任務需手動執行 API 測試 -->
