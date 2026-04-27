## ADDED Requirements

### Requirement: 使用者模型擴充與 Identity 整合
系統需使用 `ApplicationUser` 繼承自 `IdentityUser`，並包含在 `OrderDbContext` 中，以便未來擴充會員資訊。

#### Scenario: 產生包含身分驗證資料表的資料庫
- **WHEN** EF Core 執行 Initial Migration 時
- **THEN** 會自動產生 ASP.NET Core Identity 所需的資料表 (AspNetUsers, AspNetRoles 等) 並確保主鍵、索引正確建立。

### Requirement: 商品 (Product) 管理模型
系統需有一個 `Product` 實體，供管理員管理上架與下架。

#### Scenario: 商品屬性定義
- **WHEN** 建立 Product 實體
- **THEN** 必須包含 Id, Name, Description, Price, Stock, IsActive, CreatedAt, UpdatedAt 等屬性，且 Price 在資料庫層級應定義精確度 (如 decimal(18,2))。

### Requirement: 訂單 (Order) 與訂單明細 (OrderItem) 模型
使用者下單時需記錄訂單主檔與明細，且明細中需鎖定當時的單價。

#### Scenario: 訂單與使用者的關聯
- **WHEN** 建立 Order 實體
- **THEN** 必須包含 UserId 外鍵關聯至 `ApplicationUser`，確保可以追蹤是哪位使用者下的訂單。

#### Scenario: 訂單明細與商品的關聯
- **WHEN** 建立 OrderItem 實體
- **THEN** 必須包含 OrderId 關聯至 Order，以及 ProductId 關聯至 Product，並包含 Quantity (數量) 與 UnitPrice (結帳時的單價)。
