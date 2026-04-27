## Why

為了建構代購平台，我們需要先定義底層的資料庫結構與關聯模型。透過使用 EF Core 與 `IdentityDbContext`，我們可以快速建立會員管理系統，並擴充代購業務所需的各項實體（如商品、訂單、訂單明細等）。這個提案目的是釐清並建立這些核心資料表及其關聯，作為後續開發 API 的基礎。

## What Changes

建立代購平台的核心資料庫實體與關聯：
1. **身分驗證與角色 (Identity & Role)**：區分管理員 (Admin) 與使用者 (User)。
2. **商品管理 (Product)**：管理員可以上架、下架代購商品。
3. **訂單管理 (Order & OrderItem)**：使用者可以針對上架商品建立訂單，並記錄購買明細。
4. 設定 DbContext 內的 DbSet 與實體關聯 (Fluent API)。

## Capabilities

### New Capabilities
- `proxy-platform-data-model`: 建立代購平台所需的核心資料表（User, Product, Order, OrderItem）與關聯設計。

### Modified Capabilities

## Impact

- `OrderSystem.Api/Data/OrderDbContext.cs` 將加入多個新的 `DbSet`。
- 新增多個 Entity 模型檔案。
- 將會產生新的 EF Core Migration 檔案。
