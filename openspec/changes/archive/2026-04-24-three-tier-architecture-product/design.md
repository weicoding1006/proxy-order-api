## Context

本專案採用三層式架構（Three-Tier Architecture）作為標準設計模式，以 Product 模組為範例實作。三層分別為：

1. **Presentation Layer（表現層）** — `ProductController`：負責接收 HTTP 請求、驗證輸入格式、回傳標準化回應
2. **Business Logic Layer（業務邏輯層）** — `ProductService`：封裝所有業務規則與驗證，協調資料流
3. **Data Access Layer（資料存取層）** — `ProductRepository`：負責所有 ORM/資料庫操作，隔離持久化細節

各層之間透過 DTO 傳遞資料，透過 Interface 定義契約，確保低耦合、高內聚。

## Goals / Non-Goals

**Goals:**
- 建立清晰的層次分離，每層只負責單一職責
- Controller 不含業務邏輯；Service 不直接存取資料庫；Repository 不含業務規則
- 各層可獨立進行單元測試（Service 可 mock Repository）
- 提供可複製至其他模組（Order、User 等）的架構範本

**Non-Goals:**
- 不處理認證/授權（Auth 為另一模組）
- 不實作快取、分頁、或進階查詢（可作為後續擴展）
- 不規定特定框架（NestJS / Express 均可套用此模式）

## Decisions

### 決策 1：DTO 與 Entity 分離

**選擇**: 使用獨立的 DTO 類別（`CreateProductDto`、`UpdateProductDto`、`ProductResponseDto`），不直接暴露 Entity

**理由**: Entity 與資料庫 schema 綁定，若直接回傳 Entity 會導致：
- API 契約與資料庫 schema 耦合
- 敏感欄位（如 `deletedAt`、內部 ID）可能意外暴露

**替代方案考慮**: 直接回傳 Entity → 拒絕，因為破壞層間隔離原則

---

### 決策 2：Repository Interface 抽象

**選擇**: 定義 `IProductRepository` Interface，Service 依賴 Interface 而非具體實作

**理由**:
- 方便替換資料庫（TypeORM → Prisma）而不影響 Service
- 單元測試時可輕易 mock Repository

**替代方案考慮**: 直接注入具體 Repository 類別 → 拒絕，違反依賴反轉原則（DIP）

---

### 決策 3：錯誤處理集中在 Service 層

**選擇**: Service 負責拋出業務例外（如 `ProductNotFoundException`），Controller 只負責將例外轉換為 HTTP 狀態碼

**理由**: 業務規則（「產品不存在」）屬於業務層知識，不應散落在 Controller

## Risks / Trade-offs

| 風險 | 緩解策略 |
|------|----------|
| 過多的 DTO 類別增加維護成本 | 只為真正有差異的場景建立 DTO，共用 Response DTO |
| 三層呼叫鏈增加一層間接性 | 對於簡單 CRUD，這是可接受的 trade-off；複雜查詢仍可在 Repository 優化 |
| Interface 定義與實作不同步 | TypeScript 靜態型別檢查可在編譯期捕捉不一致 |

## Migration Plan

1. 建立 `Product` Entity / Model 定義
2. 建立 `IProductRepository` Interface 與 `ProductRepository` 實作
3. 建立 `ProductService`，注入 `IProductRepository`
4. 建立各 DTO 類別
5. 建立 `ProductController`，注入 `ProductService`
6. 撰寫各層單元測試
7. 撰寫整合測試（Controller → Service → Repository → DB）

**Rollback**: 各層獨立，可個別回滾而不影響其他層。

## Open Questions

- 框架選擇：NestJS（推薦，內建 DI 容器）或純 Express？
- ORM 選擇：TypeORM 或 Prisma？
- 是否需要軟刪除（Soft Delete）支援？
