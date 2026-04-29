## Why

新進開發者或維護人員進入專案時，缺乏一份說明各層職責的文件，難以快速了解四個專案資料夾的分工。透過新增一份繁體中文架構說明文件，讓團隊成員能在幾分鐘內掌握整體架構。

## What Changes

- 在 `docs/` 目錄新增 `architecture.md`，以繁體中文說明四個專案資料夾（Domain、Application、Infrastructure、Api）各自的職責、包含的類型，以及彼此的依賴關係。

## Capabilities

### New Capabilities

- `architecture-docs`: 架構說明文件，涵蓋四層各自的職責與內容，放置於 `docs/architecture.md`。

### Modified Capabilities

（無）

## Impact

- 新增 `docs/architecture.md` 一份文件；不影響任何程式碼或 API 行為。
