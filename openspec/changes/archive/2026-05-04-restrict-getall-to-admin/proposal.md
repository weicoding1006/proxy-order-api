## Why

`GET /api/orders/all` 目前僅受 `[Authorize]` 保護，任何已登入使用者皆可取得全平台所有訂單，造成隱私與資安風險。應將此端點限制為僅 `Admin` 角色可存取，符合最小權限原則。

## What Changes

- `GET /api/orders/all` 端點加上 `[Authorize(Roles = "Admin")]`，非 Admin 使用者呼叫將回傳 `403 Forbidden`
- 更新 `order-controller` spec，補充 `GET /api/orders/all` 的存取控制需求與情境

## Capabilities

### New Capabilities
<!-- 無新 capability，僅修改現有端點行為 -->

### Modified Capabilities
- `order-controller`: 新增 `GET /api/orders/all` 端點的 Admin 角色存取限制需求

## Impact

- **程式碼**：`OrderSystem.Api/Controllers/OrdersController.cs` — `GetAll` action 加上 role-based authorization attribute
- **API**：現有非 Admin 使用者呼叫 `GET /api/orders/all` 將從 `200 OK` 變為 `403 Forbidden`（**BREAKING** for non-admin callers）
- **依賴**：依賴既有 ASP.NET Core Identity 角色機制，無需新套件
