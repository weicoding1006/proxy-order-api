## Context

我們正在建置一個代購平台。根據需求，系統分為「管理員」與「一般使用者」兩種角色。管理員負責商品的上下架管理，一般使用者則可以瀏覽商品並建立訂單。為支援此需求，我們決定採用 ASP.NET Core Identity 搭配 Entity Framework Core (`IdentityDbContext`) 來處理身分驗證與授權，同時擴充我們業務邏輯所需的核心實體（Product, Order, OrderItem）。

## Goals / Non-Goals

**Goals:**
- 建立並設定自訂的 `ApplicationUser` 以繼承 `IdentityUser`。
- 建立 `Product` (商品)、`Order` (訂單)、`OrderItem` (訂單明細) 實體模型。
- 定義實體之間的關聯 (Relations)，例如 User-Order (1:N), Order-OrderItem (1:N), Product-OrderItem (1:N)。
- 在 `OrderDbContext` 中加入這些 `DbSet` 並透過 Fluent API 設定欄位限制與關聯。

**Non-Goals:**
- 實作 API 路由、Controller 或 Service 商業邏輯。
- 實作前端畫面。
- 第三方金流或物流整合（目前僅建立基礎資料表）。

## Decisions

1. **使用者身分繼承 (`ApplicationUser`)**: 
   不直接使用內建的 `IdentityUser`，而是建立 `ApplicationUser` 類別並繼承之。此舉可保留未來擴充欄位（例如：姓名、地址、註冊時間等）的彈性。

2. **核心資料表設計與關聯**:
   - `Product` (商品): 
     - 欄位：Id, Name, Description, Price, Stock, IsActive (是否上架), CreatedAt, UpdatedAt。
   - `Order` (訂單):
     - 欄位：Id, UserId (FK -> ApplicationUser), TotalAmount, Status (Pending, Paid, Shipped, etc.), CreatedAt。
     - 關聯：一個 User 可以有多筆 Order。
   - `OrderItem` (訂單明細):
     - 欄位：Id, OrderId (FK -> Order), ProductId (FK -> Product), Quantity, UnitPrice。
     - 關聯：一個 Order 可以有多個 OrderItem；一個 OrderItem 對應一個 Product。

3. **使用 Fluent API 設定約束 (Constraints)**:
   在 `OrderDbContext.OnModelCreating` 中設定 decimal 的精確度（例如 `decimal(18,2)` 給價格）、字串的最大長度，以及明確的外鍵約束，保持實體類別的乾淨。

## Risks / Trade-offs

- **風險**: 忘記在 `OnModelCreating` 中呼叫 `base.OnModelCreating(builder)` 會導致 Identity 資料表無法正確產生主鍵與關聯，進而造成系統報錯。
  **緩解措施**: 在設計與開發任務中明確要求將 `base.OnModelCreating(builder)` 放在覆寫方法的第一行。

- **風險**: 商品價格變動導致歷史訂單總價計算錯誤。
  **緩解措施**: 在 `OrderItem` 實體中存入 `UnitPrice`，當下單時將當下的商品價格快照 (Snapshot) 存入明細中，而非直接關聯查價。
