## Context

專案採用四層式清晰架構（Clean Architecture），但目前 `docs/` 目錄內只有 `entity-relationships.md` 與 `secrets-setup.md`，缺乏整體架構的層次說明。

## Goals / Non-Goals

**Goals:**
- 新增一份 `docs/architecture.md`，以繁體中文說明各層職責

**Non-Goals:**
- 不修改任何程式碼
- 不產生 API 文件或資料庫 schema 文件

## Decisions

### 放置於 docs/ 而非 README
`docs/` 目錄已有其他說明文件（entity-relationships.md、secrets-setup.md），架構文件應與其放在一起保持一致性。

### 使用繁體中文
依照使用者需求，文件全程使用繁體中文撰寫。
