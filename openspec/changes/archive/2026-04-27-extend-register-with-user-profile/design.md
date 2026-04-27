## Context

ASP.NET Core Identity 的 `MapIdentityApi<TUser>()` 自動掛載的 `/register` 使用 sealed 的 `RegisterRequest` record（僅含 `email`、`password`），無法擴充欄位。`ApplicationUser` 已有 `FirstName`、`LastName`，但內建路由無法在註冊時寫入這些欄位。

專案現有 spec（`identity-auth-apis`）已將目標路由定義為 `POST /api/auth/register`，與 `MapIdentityApi` 掛載的 `/register` 路徑完全不同，**不存在路由衝突**。

## Goals / Non-Goals

**Goals:**
- 建立自訂 `POST /api/auth/register` endpoint，接受 `email`、`password`、`firstName`、`lastName`
- 使用 `UserManager<ApplicationUser>` 建立使用者並寫入 `FirstName`、`LastName`
- 保留 `MapIdentityApi` 提供的其他端點（`/login`、`/refresh` 等）不動

**Non-Goals:**
- 電子郵件驗證流程（email confirmation）
- 自訂 JWT 邏輯（登入仍使用 Identity Bearer）
- 角色指派於註冊流程中

## Decisions

### 1. 自訂 Controller 而非 Minimal API

**決定**：新增 `AuthController`（Api 層 Controllers 目錄）。

**原因**：專案其他端點（`ProductController`）均採用 Controller 風格，保持一致性。

**替代方案**：Minimal API `app.MapPost("/api/auth/register", ...)` — 可行，但與現有風格不一致。

### 2. 服務層薄化

**決定**：`AuthController` 直接注入 `UserManager<ApplicationUser>`，不另建 `IAuthService`。

**原因**：目前只有一個操作（Register），引入 Service 層會增加間接層而無實質收益。若日後需加入 email confirmation 等邏輯再重構。

### 3. DTO 放置位置

**決定**：`RegisterRequest` DTO 放於 `OrderSystem.Application` 層的 `DTOs/Auth/` 目錄。

**原因**：與現有 `ProductDto`、`CreateProductRequest` 的慣例一致。

### 4. firstName / lastName 為必填

**決定**：DTO 上加 `[Required]`，空字串視為不合法。

**原因**：`ApplicationUser` 目前預設 `string.Empty`，允許空值會讓資料不完整。

## Risks / Trade-offs

- **MapIdentityApi 的 /register 仍存在**：路徑為 `/register`（無 `/api/auth/` 前綴），不會衝突，但 Scalar 文件中會同時顯示，可能讓 API 使用者混淆。→ 後續可用 OpenAPI filter 將其隱藏，非本次範圍。
- **密碼驗證一致性**：`UserManager` 套用 `IdentityOptions` 密碼原則，行為與內建端點一致。

## Migration Plan

1. 新增 `RegisterRequest` DTO（`OrderSystem.Application/DTOs/Auth/`）
2. 新增 `AuthController`（`OrderSystem.Api/Controllers/`），路由 `POST /api/auth/register`
3. `Program.cs` **無需修改**
4. 測試：呼叫 `POST /api/auth/register`，確認 `FirstName`、`LastName` 已寫入 `AspNetUsers`
5. Rollback：刪除 `AuthController` 即可，原 `/register` 端點不受影響
