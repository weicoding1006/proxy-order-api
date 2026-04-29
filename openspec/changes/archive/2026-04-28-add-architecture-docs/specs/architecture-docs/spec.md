## ADDED Requirements

### Requirement: 架構說明文件存在於 docs 目錄
`docs/architecture.md` 文件 SHALL 存在，且以繁體中文說明四個專案資料夾（Domain、Application、Infrastructure、Api）各自的職責與包含的主要類型。

#### Scenario: 文件可被找到
- **WHEN** 開發者瀏覽 `docs/` 目錄
- **THEN** SHALL 看到 `architecture.md` 文件

### Requirement: 文件說明 Domain 層
文件 SHALL 說明 `OrderSystem.Domain` 的職責：定義核心業務實體與規則，不依賴任何其他層。

#### Scenario: Domain 層內容完整
- **WHEN** 開發者閱讀 architecture.md 中關於 Domain 的段落
- **THEN** SHALL 了解該層包含 Entities（ApplicationUser、Order、OrderItem、Product）及 Exceptions，且不依賴外部框架

### Requirement: 文件說明 Application 層
文件 SHALL 說明 `OrderSystem.Application` 的職責：封裝業務邏輯、定義 Repository 介面，協調 Domain 與 Infrastructure 之間的互動。

#### Scenario: Application 層內容完整
- **WHEN** 開發者閱讀 architecture.md 中關於 Application 的段落
- **THEN** SHALL 了解該層包含 Services、Interfaces（Repository 抽象）及 DTOs

### Requirement: 文件說明 Infrastructure 層
文件 SHALL 說明 `OrderSystem.Infrastructure` 的職責：實作 Application 層定義的介面，處理資料庫存取（EF Core + PostgreSQL）。

#### Scenario: Infrastructure 層內容完整
- **WHEN** 開發者閱讀 architecture.md 中關於 Infrastructure 的段落
- **THEN** SHALL 了解該層包含 DbContext、Migrations 及 Repository 實作

### Requirement: 文件說明 Api 層
文件 SHALL 說明 `OrderSystem.Api` 的職責：作為應用程式進入點，接收 HTTP 請求並委派給 Application 層處理。

#### Scenario: Api 層內容完整
- **WHEN** 開發者閱讀 architecture.md 中關於 Api 的段落
- **THEN** SHALL 了解該層包含 Controllers、Program.cs（DI 注冊、中介軟體設定）及 appsettings

### Requirement: 文件說明層次間的依賴方向
文件 SHALL 以圖示或文字說明依賴方向：Api → Application → Domain，Infrastructure → Domain，Infrastructure 實作 Application 介面。

#### Scenario: 依賴方向清楚呈現
- **WHEN** 開發者閱讀依賴關係段落
- **THEN** SHALL 能判斷修改某一層時哪些層可能受影響
