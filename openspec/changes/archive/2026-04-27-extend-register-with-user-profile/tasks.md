## 1. DTO

- [x] 1.1 建立 `OrderSystem.Application/DTOs/Auth/` 目錄
- [x] 1.2 新增 `RegisterRequest.cs`，包含 `Email`、`Password`、`FirstName`、`LastName`，全部加上 `[Required]`

## 2. Controller

- [x] 2.1 新增 `OrderSystem.Api/Controllers/AuthController.cs`，路由前綴 `api/auth`
- [x] 2.2 注入 `UserManager<ApplicationUser>`
- [x] 2.3 實作 `POST /api/auth/register`：建立 `ApplicationUser`，寫入 `FirstName`、`LastName`，呼叫 `UserManager.CreateAsync`
- [x] 2.4 成功回傳 `201 Created`；失敗回傳 `400 Bad Request` 含 Identity 錯誤清單

## 3. 驗證

- [x] 3.1 使用 Scalar 或 curl 呼叫 `POST /api/auth/register`，確認 `201 Created` 且 DB 中 `FirstName`、`LastName` 有值
- [x] 3.2 確認缺少必填欄位時回傳 `400` 含 validation 訊息
- [x] 3.3 確認重複 email 時回傳 `400` 含 Identity 錯誤
<!-- 手動驗證任務，需重啟 API 後執行 -->
