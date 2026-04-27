# Entity Relationships：User / Order / OrderItem / Product

## 關係圖

```
ApplicationUser (使用者)
    │
    │ 一個使用者可以有很多筆訂單
    ▼
Order (訂單主檔)
    │  UserId ──────────────→ ApplicationUser.Id
    │
    │ 一張訂單可以有很多筆明細
    ▼
OrderItem (訂單明細)
    │  OrderId ─────────────→ Order.Id
    │  ProductId ────────────→ Product.Id
    ▼
Product (商品，獨立存在)
```

## 具體例子

你下了一張訂單，同時買了「滑鼠」和「鍵盤」：

| 表格 | 資料 |
|------|------|
| **Order** | Id=A001, UserId=我, TotalAmount=1500, Status=Pending |
| **OrderItem** | Id=X, OrderId=A001, ProductId=滑鼠, Qty=1, UnitPrice=500 |
| **OrderItem** | Id=Y, OrderId=A001, ProductId=鍵盤, Qty=2, UnitPrice=500 |
| **Product** | Id=滑鼠, Name="滑鼠", Price=500, Stock=10 |
| **Product** | Id=鍵盤, Name="鍵盤", Price=500, Stock=5 |

## 常見問題

### 為什麼需要 OrderItem，不直接在 Order 裡記商品？

因為**一張訂單可以買多樣商品**。如果直接存在 Order 裡，就只能買一種。  
OrderItem 是「訂單」和「商品」之間的橋接表（many-to-many 中間表）。

### 為什麼 OrderItem 要記 UnitPrice，不直接查 Product.Price？

`Product.Price` 會隨時間變動（漲價、促銷）。`OrderItem.UnitPrice` 是**下單當下的價格快照**，確保一年後查舊訂單，金額仍然正確，不受商品改價影響。

## C# 結構速覽

```
User
 └── Orders[]          ← 一個 User 有多張 Order
      └── OrderItems[] ← 一張 Order 有多筆 OrderItem
           └── Product ← 每筆 OrderItem 指向一個 Product
```

## 對應的 EF Core 關聯設定（OrderDbContext）

```csharp
// Order ↔ ApplicationUser（多對一）
entity.HasOne(e => e.User)
      .WithMany(u => u.Orders)
      .HasForeignKey(e => e.UserId);

// OrderItem ↔ Order（多對一）
entity.HasOne(e => e.Order)
      .WithMany(o => o.OrderItems)
      .HasForeignKey(e => e.OrderId);

// OrderItem ↔ Product（多對一）
entity.HasOne(e => e.Product)
      .WithMany()
      .HasForeignKey(e => e.ProductId);
```
