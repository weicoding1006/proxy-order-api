### Requirement: Seed roles on startup
應用程式啟動時 SHALL 確保 `Admin` 與 `User` 兩個角色存在於 `AspNetRoles`。若角色已存在則跳過，不重複建立（idempotent）。

#### Scenario: Roles seeded on first run
- **WHEN** 應用程式首次啟動且 `AspNetRoles` 為空
- **THEN** `Admin` 和 `User` 角色 SHALL 存在於資料庫

#### Scenario: Seeder is idempotent
- **WHEN** 應用程式重新啟動且角色已存在
- **THEN** 不應重複建立角色，啟動 SHALL 正常完成不拋出例外

### Requirement: Seed default Admin account on startup
若設定檔包含 `DefaultAdmin:Email` 與 `DefaultAdmin:Password`，應用程式啟動時 SHALL 確保該帳號存在並具備 `Admin` 角色。若帳號已存在則跳過。

#### Scenario: Default admin created on first run
- **WHEN** `DefaultAdmin:Email` 和 `DefaultAdmin:Password` 已設定且該 email 不存在於 `AspNetUsers`
- **THEN** 該帳號 SHALL 被建立並賦予 `Admin` 角色

#### Scenario: Seed skipped when config is absent
- **WHEN** `DefaultAdmin:Email` 或 `DefaultAdmin:Password` 未設定
- **THEN** 啟動 SHALL 正常完成，不拋出例外，不建立任何帳號

#### Scenario: Seed skipped when admin already exists
- **WHEN** `DefaultAdmin:Email` 對應的帳號已存在於 `AspNetUsers`
- **THEN** 不重複建立，啟動 SHALL 正常完成
