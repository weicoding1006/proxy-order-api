## Why

`MapIdentityApi<ApplicationUser>()` 自動產生的 `/register` endpoint 內部使用 ASP.NET Core Identity 固定的 `RegisterRequest` record（只有 `email` + `password`），無法直接傳入 `ApplicationUser` 自訂欄位（`FirstName`、`LastName`）。需要以自訂 Controller 取代，讓註冊時即可一併儲存完整使用者資料。

## What Changes

- 新增 `POST /api/auth/register` 自訂端點，接受 `{ email, password, firstName, lastName }`
- 新增 `RegisterRequest` DTO（位於 Application 層）
- 新增 `AuthController`，使用 `UserManager<ApplicationUser>` 建立使用者並寫入 `FirstName`、`LastName`
- **BREAKING**：原本由 `MapIdentityApi` 產生的 `/register` 路由將被停用或覆蓋，改由 `AuthController` 負責

## Capabilities

### New Capabilities
- `custom-register-endpoint`: 自訂註冊端點，支援 `firstName`、`lastName` 等額外欄位，回傳 `201 Created` 或 `400 Bad Request`

### Modified Capabilities
- `identity-auth-apis`: 更新「User registration endpoint」需求，將可接受欄位從 `{ email, password }` 擴充為 `{ email, password, firstName, lastName }`，且端點實作改為自訂 Controller 而非 Identity 內建路由

## Impact

- **API**：`POST /register`（Identity 內建）停用；改用 `POST /api/auth/register`（自訂）
- **Application 層**：新增 `RegisterRequest` DTO、`IAuthService` 介面與 `AuthService` 實作（呼叫 `UserManager`）
- **Api 層**：新增 `AuthController`
- **Program.cs**：`MapIdentityApi` 可保留（提供 `/login`、`/refresh` 等），但需確認 `/register` 路由不衝突
