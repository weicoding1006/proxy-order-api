## Context

`AspNetRoles` 目前為空，`Admin`/`User` 角色不存在。`RolesController` 的所有端點都需要 Admin 角色，造成無法自助建立的僵局。此外，Admin 目前無法查詢其他使用者的 id，導致角色指派流程無法完成。

## Goals / Non-Goals

**Goals:**
- 啟動時確保 `Admin` 和 `User` 角色存在（idempotent）
- 啟動時確保預設 Admin 帳號存在並具備 Admin 角色
- 使用者註冊後自動獲得 `User` 角色
- Admin 可透過 `GET /api/users` 查詢所有使用者（id、email、roles）以進行後續角色指派

**Non-Goals:**
- 動態管理角色清單（角色固定為 Admin / User）
- 使用者的 CRUD（新增、刪除、更新帳號）
- 細粒度的 RBAC 權限控制

## Decisions

**Seeder 以靜態 async 方法在 `Program.cs` 的 `app.Run()` 前呼叫**

保證 seed 完成後 API 才開始接受請求。比 `IHostedService` 時序更明確，實作也更簡單。

Alternative: EF Core `HasData`。Rejected — Identity 物件有 password hash 需求，不適合 migration seed。

**預設 Admin 帳密從 user-secrets / 環境變數讀取**

Key 為 `DefaultAdmin:Email` 和 `DefaultAdmin:Password`。未設定則跳過 Admin 帳號 seed（角色仍建立）。不 hardcode 進程式碼或版控。

**Seeder 放在 `OrderSystem.Api/Seeding/` 層**

Seeder 是啟動邏輯，不是業務邏輯，不需要進 Infrastructure。

**`GET /api/users` 回傳 `{ id, email, roles }` 陣列**

roles 從 `UserManager.GetRolesAsync` 取得（當下資料庫值，非 JWT 快照）。N+1 查詢在使用者量極小的情境下可接受，不做額外優化。

## Risks / Trade-offs

- [預設 Admin 密碼外洩] → user-secrets（開發）+ 環境變數（生產），不進版控
- [Seeder 失敗導致啟動中斷] → 讓例外往上拋，啟動失敗比帶著錯誤配置運行更安全
- [`GET /api/users` N+1 查詢] → 使用者量極少（代購平台規模），可接受；若未來需要優化可改用 JOIN query
