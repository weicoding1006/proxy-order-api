namespace OrderSystem.Domain.Entities;

public class Order
{
    /// <summary>訂單唯一識別碼</summary>
    public Guid Id { get; set; }

    /// <summary>下單使用者的 ID（外鍵）</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>導覽屬性：下單使用者</summary>
    public ApplicationUser User { get; set; } = null!;

    /// <summary>訂單總金額</summary>
    public decimal TotalAmount { get; set; }

    /// <summary>訂單狀態（預設：Pending 待處理）</summary>
    public string Status { get; set; } = "Pending";

    /// <summary>訂單建立時間</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>訂單所含的所有明細（一對多）</summary>
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
