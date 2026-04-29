## ADDED Requirements

### Requirement: Admin can list all users
應用程式 SHALL 暴露 `GET /api/users`，僅限具備 `Admin` 角色的已認證使用者呼叫。回傳所有使用者的陣列，每筆包含 `id`、`email`、`roles`。

#### Scenario: Admin retrieves user list
- **WHEN** 具備 `Admin` 角色的使用者呼叫 `GET /api/users`
- **THEN** 回應 SHALL 為 `200 OK`
- **AND** body SHALL 為 JSON 陣列，每筆包含 `id`（string）、`email`（string）、`roles`（string array）

#### Scenario: Non-admin is forbidden
- **WHEN** 具備 `User` 角色但不具備 `Admin` 角色的使用者呼叫 `GET /api/users`
- **THEN** 回應 SHALL 為 `403 Forbidden`

#### Scenario: Unauthenticated request is rejected
- **WHEN** 未附帶 Bearer token 呼叫 `GET /api/users`
- **THEN** 回應 SHALL 為 `401 Unauthorized`
