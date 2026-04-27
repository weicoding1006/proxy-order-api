## 1. 實體模型建立 (Entity Models Creation)

- [x] 1.1 建立 `ApplicationUser` 類別，繼承自 `IdentityUser`，並加入所需的額外屬性（保留未來擴充空間）。
- [x] 1.2 建立 `Product` 模型，包含 Id, Name, Description, Price, Stock, IsActive, CreatedAt, UpdatedAt。
- [x] 1.3 建立 `Order` 模型，包含 Id, UserId, TotalAmount, Status, CreatedAt，並建立與 `ApplicationUser` 的關聯。
- [x] 1.4 建立 `OrderItem` 模型，包含 Id, OrderId, ProductId, Quantity, UnitPrice，並建立與 `Order`、`Product` 的導覽屬性 (Navigation Properties)。

## 2. DbContext 設定與關聯 (DbContext & Relations)

- [x] 2.1 將 `OrderDbContext` 的泛型基礎類別改為 `IdentityDbContext<ApplicationUser>`。
- [x] 2.2 在 `OrderDbContext` 中加入 `Products`, `Orders`, `OrderItems` 的 `DbSet` 屬性。
- [x] 2.3 覆寫 `OnModelCreating` 方法，確保第一行呼叫 `base.OnModelCreating(builder)`。
- [x] 2.4 在 `OnModelCreating` 內使用 Fluent API 定義各實體間的關聯 (1:N 等) 與欄位約束（如 decimal 精度限制）。

## 3. 服務註冊與資料庫更新 (Service Registration & Migration)

- [x] 3.1 檢查並更新 `Program.cs`，確認 `AddDbContext` 與 `AddIdentity` 正確綁定 `ApplicationUser` 與 `IdentityRole`。
- [x] 3.2 使用命令列執行 `dotnet ef migrations add InitialIdentitySetup` 以產生 Migration。
- [x] 3.3 使用命令列執行 `dotnet ef database update` 將變更套用至資料庫。
