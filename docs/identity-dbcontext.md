# IdentityDbContext 與 User/Role 資料表的協作機制

## 核心概念

`IdentityDbContext<TUser>` 是 ASP.NET Core Identity 提供的特殊 DbContext 基底類別。它替你預先定義好了所有 Identity 需要的資料表（User、Role、關聯表），你只需繼承它，就不用自己寫這些 Entity 設定。

本專案的 `OrderDbContext` 就是這樣做的：

```csharp
public class OrderDbContext(DbContextOptions<OrderDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    // 你自己的業務資料表
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
}
```

---

## IdentityDbContext 幫你產生哪些資料表

繼承 `IdentityDbContext<ApplicationUser>` 後，EF Core Migration 會自動建立以下資料表：

| 資料表               | 用途                                 |
|----------------------|--------------------------------------|
| `AspNetUsers`        | 使用者帳號（對應 `ApplicationUser`） |
| `AspNetRoles`        | 角色定義（如 Admin、Member）         |
| `AspNetUserRoles`    | 使用者 ↔ 角色 的多對多關聯表        |
| `AspNetUserClaims`   | 使用者自訂 Claim                     |
| `AspNetRoleClaims`   | 角色層級的 Claim                     |
| `AspNetUserLogins`   | 第三方登入（Google、Facebook 等）    |
| `AspNetUserTokens`   | Token 儲存（如密碼重設 token）       |

這些資料表的欄位、索引、外鍵，全部在 `base.OnModelCreating(builder)` 裡由 Identity 框架自動設定好，所以 `OnModelCreating` 必須先呼叫 `base`，再疊加你自己的設定：

```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);  // ← 先讓 Identity 把它的 Entity 設定好
    // 再設定你自己的 Product、Order、OrderItem...
}
```

---

## ApplicationUser 如何擴充 IdentityUser

`ApplicationUser` 繼承 `IdentityUser`，`IdentityUser` 本身已包含：

- `Id`（string GUID）
- `UserName`、`Email`、`PasswordHash`
- `PhoneNumber`、`EmailConfirmed`、`LockoutEnd` 等安全欄位

本專案在此基礎上額外加了三個業務欄位，這些欄位會直接出現在 `AspNetUsers` 資料表裡：

```csharp
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Order> Orders { get; set; }  // 業務關聯
}
```

---

## User 與 Role 如何協同

### 資料層（資料表關係）

```
AspNetUsers          AspNetUserRoles         AspNetRoles
───────────          ───────────────         ───────────
Id (PK)  ◄──────── UserId (FK)              Id (PK)
UserName             RoleId (FK) ────────► Name
Email                                        NormalizedName
...
```

`AspNetUserRoles` 是純粹的多對多 join table，沒有額外欄位，只有 `UserId` + `RoleId` 兩個外鍵。

### 應用層（UserManager / RoleManager）

你不會直接去 SQL query 這些表，而是透過 Identity 提供的 Manager 類別操作：

| 操作 | 使用哪個 Manager |
|------|-----------------|
| 建立 / 查詢使用者 | `UserManager<ApplicationUser>` |
| 建立角色 | `RoleManager<IdentityRole>` |
| 把角色指派給使用者 | `UserManager.AddToRoleAsync()` |
| 取得使用者的角色 | `UserManager.GetRolesAsync()` |
| 驗證密碼 | `UserManager.CheckPasswordAsync()` |

這些 Manager 都是由 DI 注入的，背後透過 `OrderDbContext` 存取資料庫。

---

## 本專案的 DI 註冊鏈

`Program.cs` 的這段程式碼把所有東西串起來：

```csharp
builder.Services.AddIdentityCore<ApplicationUser>(options => { ... })
    .AddRoles<IdentityRole>()              // 啟用角色功能，註冊 RoleManager
    .AddEntityFrameworkStores<OrderDbContext>()  // 告訴 Identity 用 OrderDbContext 存資料
    .AddDefaultTokenProviders();           // 啟用密碼重設 token 等功能
```

- `AddIdentityCore` — 核心服務，含 `UserManager`
- `.AddRoles<IdentityRole>()` — 加入 `RoleManager`，Identity 才知道要管理 `AspNetRoles` 表
- `.AddEntityFrameworkStores<OrderDbContext>()` — 把 `UserManager`、`RoleManager` 的儲存層接到 `OrderDbContext`（也就是你的 SQLite/PostgreSQL）

沒有 `.AddEntityFrameworkStores`，UserManager 就不知道要去哪裡讀寫資料。

---

## 整體資料流（以登入為例）

```
POST /api/auth/login
       │
       ▼
AuthController.Login()
       │  userManager.FindByEmailAsync(email)
       ▼
UserManager ──► OrderDbContext ──► AspNetUsers 資料表
       │
       │  userManager.CheckPasswordAsync(user, password)
       ▼
UserManager 用 PasswordHasher 比對 PasswordHash 欄位
       │
       │  userManager.GetRolesAsync(user)
       ▼
UserManager ──► OrderDbContext ──► AspNetUserRoles JOIN AspNetRoles
       │
       ▼
產生 JWT（含 sub、email、role claims）
```

---

## 小結

| 元件 | 職責 |
|------|------|
| `IdentityDbContext<ApplicationUser>` | 提供所有 Identity 資料表的 Entity 設定 |
| `ApplicationUser : IdentityUser` | 擴充使用者欄位，對應 `AspNetUsers` |
| `UserManager<ApplicationUser>` | 使用者的 CRUD 與驗證操作（不寫 SQL，走 DbContext）|
| `RoleManager<IdentityRole>` | 角色的 CRUD 操作 |
| `AspNetUserRoles` | User ↔ Role 的多對多關聯，由 Identity 自動管理 |
| `base.OnModelCreating(builder)` | 讓 Identity 把上述所有 Entity mapping 注入進來 |
