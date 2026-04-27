namespace OrderSystem.Domain.Entities;

public class OrderItem
{
    /// <summary>訂單明細唯一識別碼</summary>
    public Guid Id { get; set; }

    /// <summary>所屬訂單的 ID（外鍵）</summary>
    public Guid OrderId { get; set; }

    /// <summary>導覽屬性：所屬訂單</summary>
    public Order Order { get; set; } = null!;

    /// <summary>商品的 ID（外鍵）</summary>
    public Guid ProductId { get; set; }

    /// <summary>導覽屬性：對應商品</summary>
    public Product Product { get; set; } = null!;

    /// <summary>購買數量</summary>
    public int Quantity { get; set; }

    /// <summary>下單當時的單價（快照，避免商品改價後影響歷史訂單）</summary>
    public decimal UnitPrice { get; set; }
}
