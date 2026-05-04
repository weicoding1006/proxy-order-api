## 1. Controller 授權修改

- [x] 1.1 在 `OrdersController.GetAll` action 上加入 `[Authorize(Roles = "Admin")]` attribute

## 2. 驗證與測試

- [ ] 2.1 以 Admin token 呼叫 `GET /api/orders/all`，確認回傳 `200 OK`
- [ ] 2.2 以非 Admin 已登入 token 呼叫 `GET /api/orders/all`，確認回傳 `403 Forbidden`
- [ ] 2.3 不帶 token 呼叫 `GET /api/orders/all`，確認回傳 `401 Unauthorized`
