## 1. Role Seeder

- [x] 1.1 建立 `OrderSystem.Api/Seeding/RoleSeeder.cs`，包含靜態 `SeedAsync(IServiceProvider)` 方法
- [x] 1.2 在 `SeedAsync` 中使用 `RoleManager` 建立 `Admin` 與 `User` 角色（若不存在）
- [x] 1.3 在 `SeedAsync` 中讀取 `DefaultAdmin:Email` 與 `DefaultAdmin:Password`，若已設定則建立 Admin 帳號並賦予 `Admin` 角色（若帳號不存在）
- [x] 1.4 在 `Program.cs` 的 `app.Run()` 前呼叫 `await RoleSeeder.SeedAsync(app.Services)`
- [x] 1.5 在 user-secrets 加入 `DefaultAdmin:Email` 與 `DefaultAdmin:Password`

## 2. 註冊自動賦予 User 角色

- [x] 2.1 在 `AuthController.Register` 的 `userManager.CreateAsync` 成功後，加入 `await userManager.AddToRoleAsync(user, "User")`

## 3. 使用者清單 API

- [x] 3.1 建立 `OrderSystem.Api/Controllers/UsersController.cs`，路由 `api/users`，套用 `[Authorize(Roles = "Admin")]`
- [x] 3.2 實作 `GET /api/users`：取得所有使用者，對每位使用者呼叫 `GetRolesAsync`，回傳 `{ id, email, roles }` 陣列

## 4. 驗證

- [x] 4.1 重啟應用程式，確認啟動時無例外，`AspNetRoles` 已有 `Admin` 與 `User` 兩筆資料
- [x] 4.2 以預設 Admin 帳號登入，呼叫 `GET /api/users` 確認回傳使用者清單
- [x] 4.3 註冊新帳號後呼叫 `GET /api/auth/me`，確認 `roles` 包含 `"User"`
- [x] 4.4 以 Admin 身份呼叫 `POST /api/roles/{roleId}/users` 將某使用者升級為 Admin，再呼叫 `GET /api/auth/me` 確認角色已更新
