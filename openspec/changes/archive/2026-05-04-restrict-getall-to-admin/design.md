## Context

目前 `OrdersController` 整個 controller 套用 `[Authorize]`，代表所有端點只需登入即可存取。`GET /api/orders/all` 負責回傳全平台所有訂單，屬於管理員功能，但現行實作並未對角色做任何限制。專案已使用 ASP.NET Core Identity，並有 `Admin` 角色 seeding 機制（`role-seeding` spec）。

## Goals / Non-Goals

**Goals:**
- 限制 `GET /api/orders/all` 僅 `Admin` 角色可呼叫
- 非 Admin 已登入使用者收到 `403 Forbidden`
- 未登入使用者仍收到 `401 Unauthorized`

**Non-Goals:**
- 修改其他端點的授權邏輯
- 新增角色管理 API
- 變更 JWT 結構或 token 核發流程

## Decisions

### 使用 `[Authorize(Roles = "Admin")]` 覆蓋 action 層級

**選擇**：在 `GetAll` action 上加 `[Authorize(Roles = "Admin")]`，而非改動 controller 層級的 `[Authorize]`。

**理由**：Controller 層的 `[Authorize]` 保留原有「需登入」的基礎保護，action 層的 role attribute 疊加其上，形成「先驗 JWT、再驗 role」的雙層檢查，不影響其他 action。

**替代方案考量**：
- 將 controller `[Authorize]` 拆分至每個 action — 改動範圍大，引入重構風險，超出本次需求範疇。
- 使用 Policy-based authorization — 對單一角色檢查過度設計；`Roles` 參數已足夠。

### 不新增 Service/Repository 層邏輯

角色驗證屬於 HTTP 層（controller）責任，無需下沉至 Service。ASP.NET Core middleware pipeline 在進入 action 前即完成 403 回應，Service 層無感知。

## Risks / Trade-offs

- **[Risk] 角色名稱拼寫錯誤** → Mitigation：使用常數或與 `role-seeding` 中的字串保持一致（`"Admin"`），可考慮抽為常數類別。
- **[Risk] 現有整合測試未驗證 403** → Mitigation：tasks 中加入對應測試案例。
- **[Trade-off] 直接套用 Roles 而非 Policy** → 單純、可讀，但未來若需要更細緻的權限組合，需重構為 Policy；此風險在代購平台當前規模下可接受。

## Migration Plan

1. 加上 `[Authorize(Roles = "Admin")]` 於 `GetAll` action
2. 確認 `Admin` 角色已由 seeding 機制建立（現有 `role-seeding` spec 已涵蓋）
3. 確保測試環境中測試帳號有正確的角色指派
4. 部署後以非 Admin token 呼叫 `GET /api/orders/all` 驗證回傳 403

**Rollback**：移除新增的 `[Authorize(Roles = "Admin")]` attribute 即可回復，無資料庫變更。
