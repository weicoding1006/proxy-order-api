## Context

專案採 Clean Architecture 分層：Domain → Application → Infrastructure → Api。`Order`、`OrderItem`、`ApplicationUser` 實體已定義於 Domain 層，`OrderDbContext` 已完整設定關聯與精確度，Migration 已存在。現有 Product 層（IProductRepository → ProductRepository → ProductService → ProductController）是標準的實作模式，Order 層應遵循相同模式。

## Goals / Non-Goals

**Goals:**
- 補齊 Order 的 Repository、Service、DTO、Controller 四層
- 建立訂單時鎖定 UnitPrice（價格快照）並驗證庫存
- 查詢限制當前登入使用者自己的訂單（資料隔離）
- 從 JWT claims 取得 UserId，不透過額外 User API

**Non-Goals:**
- 訂單狀態流轉（取消、出貨、完成）
- 金流整合或付款流程
- 管理員跨使用者查詢訂單
- 訂單修改或刪除

## Decisions

### 1. UnitPrice 在 Service 層快照，不由 Client 傳入

**決定**：`CreateOrderRequest` 中只傳 `productId` 與 `quantity`，Service 從資料庫讀取 `Product.Price` 並寫入 `OrderItem.UnitPrice`。  
**理由**：若允許 Client 自行傳入 UnitPrice，惡意使用者可任意竄改價格；快照由 Server 決定才能確保正確性。

### 2. TotalAmount 由 Service 計算（sum of Quantity × UnitPrice）

**決定**：不由 Client 傳入，不使用資料庫觸發器，而在 `OrderService.CreateAsync` 中算出後寫入 `Order.TotalAmount`。  
**理由**：維持業務邏輯在 Application 層，易於測試與追蹤。

### 3. 庫存驗證在 Service 層，不在資料庫約束

**決定**：`OrderService` 建立訂單前檢查 `Product.Stock >= requestedQuantity`，若不足拋出 domain exception。  
**理由**：資料庫 CHECK 約束無法給出有意義的錯誤訊息；Service 層驗證可回傳明確的 400 Bad Request。  
**替代方案**：Transaction + row-level lock（適合高並發），目前流量不需要此複雜度。

### 4. UserId 從 JWT claims 讀取，不從 request body 接受

**決定**：Controller 使用 `User.FindFirstValue(ClaimTypes.NameIdentifier)` 取得 UserId，再傳給 Service。  
**理由**：與 AuthController 的 login 實作一致；防止使用者偽造他人 UserId。

### 5. 查詢單筆訂單時，若訂單不屬於當前使用者回傳 404（而非 403）

**決定**：`GET /api/orders/{id}` 若訂單存在但 UserId 不符，回傳 `404 Not Found`。  
**理由**：回傳 403 會暗示資源存在，洩漏訂單 ID 資訊；404 可防止枚舉攻擊。

### 6. IOrderRepository 介面放在 Application 層（與 IProductRepository 相同位置）

**決定**：`OrderSystem.Application/Interfaces/IOrderRepository.cs`。  
**理由**：遵循現有專案慣例，保持一致性。

## Risks / Trade-offs

- **庫存競態條件** → 目前無分散式鎖，高並發下可能超賣。Mitigation：未來可加 Pessimistic Locking 或 Outbox Pattern；現階段接受此風險。
- **TotalAmount 冗餘儲存** → 若 UnitPrice 邏輯變動，歷史訂單 TotalAmount 可能不一致。Mitigation：TotalAmount 為下單時快照，屬設計意圖，文件說明即可。
