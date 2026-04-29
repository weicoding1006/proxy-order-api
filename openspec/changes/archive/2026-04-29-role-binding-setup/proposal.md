## Why

`AspNetRoles` 目前是空的，`Admin` 和 `User` 角色不存在，導致所有角色相關 API 無法運作（雞生蛋問題）。同時，新註冊使用者沒有任何角色，且 Admin 沒有辦法查詢其他使用者的 id 來進行角色指派。

## What Changes

- 應用程式啟動時自動 seed `Admin` 與 `User` 兩個角色（若不存在）
- 應用程式啟動時自動 seed 一個預設 Admin 帳號（從 user-secrets 讀取）
- 使用者註冊成功後自動賦予 `User` 角色
- 新增 `GET /api/users`（Admin Only）列出所有使用者的 id、email、roles

## Capabilities

### New Capabilities

- `role-seeding`: 啟動時確保 `Admin`、`User` 角色與預設 Admin 帳號存在
- `user-list`: Admin 可查詢所有使用者清單（含 id、email、roles）

### Modified Capabilities

- `custom-register-endpoint`: 註冊成功後須自動將新使用者加入 `User` 角色

## Impact

- **新增** `OrderSystem.Api/Seeding/RoleSeeder.cs`
- **新增** `OrderSystem.Api/Controllers/UsersController.cs`（`GET /api/users`）
- **修改** `AuthController.Register` — 加入 `AddToRoleAsync("User")`
- **修改** `Program.cs` — 啟動後執行 RoleSeeder
- **修改** user-secrets — 新增 `DefaultAdmin:Email` 與 `DefaultAdmin:Password`
- 不需要新的 Migration
