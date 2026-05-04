# 訂單狀態機與庫存管理

## 設計概念

本系統採用**庫存軟性保留（Soft Reservation）**策略：

- 下單（`Pending`）時**保留**庫存，不正式扣除
- 管理員確認（`Confirmed`）時才**正式扣除**庫存
- 取消訂單時，依當前狀態**歸還**對應的庫存

這樣的設計讓「可用庫存」始終準確（`Stock - ReservedStock`），避免多筆訂單同時下單造成超賣。

---

## 訂單狀態流程

```
下單
  ↓
[Pending] ──── Admin 確認 ────→ [Confirmed] ──── Admin 出貨 ────→ [Shipped] ──── Admin 完成 ────→ [Completed]
  │                                  │
  └── 本人或 Admin 取消 ──→ [Cancelled]    └── Admin 取消 ──→ [Cancelled]
```

### 合法的狀態轉換

| 從 | 到 | 誰可以操作 |
|----|----|-----------|
| `Pending` | `Confirmed` | Admin |
| `Pending` | `Cancelled` | Admin 或訂單本人 |
| `Confirmed` | `Shipped` | Admin |
| `Confirmed` | `Cancelled` | Admin |
| `Shipped` | `Completed` | Admin |

> 任何不在上表內的轉換，API 會回傳 `422 Unprocessable Entity`。

---

## 庫存欄位說明

`Product` 資料表有兩個庫存相關欄位：

| 欄位 | 說明 |
|------|------|
| `Stock` | 實際總庫存，只在訂單「確認」或「取消已確認訂單」時才變動 |
| `ReservedStock` | 被 Pending 訂單佔用的庫存，下單時增加、取消或確認時釋放 |

**可用庫存 = `Stock - ReservedStock`**

### 各狀態轉換時的庫存變化

| 觸發動作 | `Stock` 變化 | `ReservedStock` 變化 |
|---------|------------|-------------------|
| 建立訂單（→ Pending） | 不變 | +Q |
| 確認訂單（Pending → Confirmed） | −Q | −Q |
| 取消 Pending 訂單 | 不變 | −Q |
| 取消 Confirmed 訂單 | +Q | 不變 |

*Q = 訂單數量*

---

## API 使用方式

### 建立訂單

```http
POST /api/orders
Authorization: Bearer <token>
Content-Type: application/json

{
  "items": [
    { "productId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "quantity": 2 }
  ]
}
```

建立成功後 `status` 一定是 `"Pending"`，同時後台會自動保留對應庫存。

---

### 更新訂單狀態

```http
PATCH /api/orders/{orderId}/status
Authorization: Bearer <token>
Content-Type: application/json

{
  "status": "Confirmed"
}
```

`status` 欄位傳入字串，對應值如下：

| 值 | 說明 |
|----|------|
| `"Confirmed"` | 確認訂單（Admin only） |
| `"Shipped"` | 標記已出貨（Admin only） |
| `"Completed"` | 標記已完成（Admin only） |
| `"Cancelled"` | 取消訂單（Admin 或訂單本人取消 Pending） |

**成功回應（200）：**

```json
{
  "id": "...",
  "userId": "...",
  "status": "Confirmed",
  "totalAmount": 1000,
  "createdAt": "...",
  "items": [...]
}
```

---

### HTTP 狀態碼一覽

| 狀態碼 | 情境 | 建議前端處理 |
|--------|------|------------|
| `200` | 狀態更新成功 | 更新畫面上的訂單狀態 |
| `400` | 庫存不足（下單時） | 顯示「庫存不足」提示 |
| `401` | 未登入或 Token 過期 | 導向登入頁 |
| `403` | 無操作權限（例如用戶想確認訂單） | 顯示「無操作權限」 |
| `404` | 訂單不存在 | 顯示「找不到訂單」 |
| `422` | 非法的狀態轉換 | 顯示 `message` 欄位的錯誤說明 |

---

## 前端 UI 建議

### 用戶端

- 訂單列表顯示 `status` badge
- 只在 `status === "Pending"` 時顯示「取消訂單」按鈕
- 其他狀態僅供查看，無操作按鈕

### 管理後台（Admin）

根據當前狀態動態顯示可操作按鈕：

| 當前狀態 | 可操作按鈕 |
|---------|-----------|
| `Pending` | 確認訂單、取消 |
| `Confirmed` | 標記出貨、取消 |
| `Shipped` | 標記完成 |
| `Completed` / `Cancelled` | 無（終態） |

點擊按鈕後呼叫 `PATCH /api/orders/{id}/status`，帶入對應的 `status` 字串即可。

---

## 常見問題

### 為什麼不在下單時直接扣 Stock？

直接扣庫存在代購場景有個問題：管理員還沒確認貨源，就已經把庫存扣掉，若訂單最終取消，還需要補回。  
保留策略讓庫存數字（`Stock`）只在「確定成交」時才變動，語意更清晰。

### 用戶下單後可以自己取消嗎？

可以，但只限 `Pending` 狀態。一旦管理員確認（`Confirmed`），就需要由管理員操作取消，避免用戶在商品已準備好後任意退訂。

### 並發下單會不會超賣？

`Product` 有 `RowVersion` 欄位做樂觀鎖。若兩筆請求同時搶最後一個庫存，只有一筆會成功，另一筆會收到 `400 庫存不足` 的錯誤。
