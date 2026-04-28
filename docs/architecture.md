# 專案架構說明

本專案採用**四層式清晰架構（Clean Architecture）**，將業務邏輯、資料存取與 API 入口明確分離，確保各層職責單一、易於測試與維護。

---

## 層次依賴方向

```
OrderSystem.Api
      │
      ▼
OrderSystem.Application  ◄──  OrderSystem.Infrastructure
      │                              │
      ▼                              ▼
OrderSystem.Domain         OrderSystem.Domain
```

- **Api** 依賴 Application（呼叫 Service）
- **Application** 依賴 Domain（使用實體與介面定義）
- **Infrastructure** 依賴 Domain（實作 Application 定義的介面）
- **Domain** 不依賴任何其他層

---

## 各層說明

### 1. OrderSystem.Domain — 領域層

> **核心業務規則，完全獨立，不依賴任何外部框架。**

這是整個架構的核心，定義系統最基本的業務概念與規則。任何一層都可以依賴它，但它本身不依賴任何其他層。

**主要內容：**

| 資料夾 | 說明 |
|--------|------|
| `Entities/` | 業務實體，代表系統中的核心物件 |
| `Exceptions/` | 自訂業務例外，描述業務規則被違反時的錯誤 |

**包含的實體：**
- `ApplicationUser` — 繼承 ASP.NET Identity 的使用者
- `Order` — 訂單主檔
- `OrderItem` — 訂單明細項目
- `Product` — 商品

---

### 2. OrderSystem.Application — 應用層

> **封裝業務邏輯，協調各層互動，定義資料存取的抽象介面。**

Application 層是業務邏輯的執行者，它知道「要做什麼」，但不關心「怎麼做」（資料庫細節由 Infrastructure 實作）。Controller 只需呼叫 Service，不直接碰資料庫。

**主要內容：**

| 資料夾 | 說明 |
|--------|------|
| `Services/` | 業務邏輯實作，處理 CRUD 操作與業務規則驗證 |
| `Interfaces/` | Repository 抽象介面，定義資料存取的合約 |
| `DTOs/` | 資料傳輸物件，用於 Controller 與 Service 之間的資料交換 |

**包含的 Services：**
- `OrderService` — 訂單建立、查詢、更新邏輯
- `ProductService` — 商品管理邏輯

**包含的 Interfaces：**
- `IOrderRepository` — 訂單資料存取合約
- `IProductRepository` — 商品資料存取合約

---

### 3. OrderSystem.Infrastructure — 基礎設施層

> **實作應用層定義的介面，處理所有與外部系統（資料庫）的互動。**

Infrastructure 層是「髒活」的地方，負責實際與 PostgreSQL 資料庫溝通。它實作 Application 層的 Repository 介面，讓業務邏輯不需要知道底層使用哪種資料庫。

**主要內容：**

| 資料夾 | 說明 |
|--------|------|
| `Data/` | EF Core 的 `DbContext`，設定資料表映射與關聯 |
| `Migrations/` | EF Core 自動產生的資料庫遷移腳本 |
| `Repositories/` | 實作 Application 層定義的 Repository 介面 |

**技術棧：**
- ORM：Entity Framework Core
- 資料庫：PostgreSQL（透過 Npgsql）
- Repository 實作：`OrderRepository`、`ProductRepository`

---

### 4. OrderSystem.Api — API 層

> **應用程式的進入點，接收 HTTP 請求，委派給 Application 層處理，回傳結果。**

Api 層是最外層，負責處理 HTTP 通訊。Controller 只做薄薄的一層：解析請求、呼叫對應的 Service、回傳 HTTP 回應，不包含任何業務邏輯。

**主要內容：**

| 資料夾／檔案 | 說明 |
|-------------|------|
| `Controllers/` | HTTP 端點定義，每個 Controller 對應一組資源 |
| `Program.cs` | 應用程式進入點，設定 DI 容器、中介軟體管線（認證、授權、Swagger）|
| `appsettings.json` | 應用程式設定（連線字串、JWT 參數等） |

**包含的 Controllers：**
- `AuthController` — 登入、註冊
- `OrdersController` — 訂單 CRUD API
- `ProductController` — 商品 CRUD API
- `RolesController` — 角色管理 API

**中介軟體設定（Program.cs）：**
- JWT Bearer 認證
- ASP.NET Identity（使用者、角色管理）
- Swagger UI（開發環境下的 API 文件介面）
- HTTPS 重新導向

---

## 新增功能時的開發流程

1. **Domain**：若需要新實體或業務規則，先在此層定義
2. **Application**：新增 Repository 介面（`Interfaces/`）、DTO、Service 方法
3. **Infrastructure**：實作 Repository 介面，必要時新增 EF Migration
4. **Api**：新增 Controller Action，注入並呼叫 Service
