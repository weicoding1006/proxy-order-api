namespace OrderSystem.Domain.Entities;

public class Product
{
    /// <summary>商品唯一識別碼</summary>
    public Guid Id { get; set; }

    /// <summary>商品名稱</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>商品描述</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>商品售價</summary>
    public decimal Price { get; set; }

    /// <summary>庫存數量</summary>
    public int Stock { get; set; }

    /// <summary>已保留但尚未正式扣除的庫存（Pending 訂單佔用）</summary>
    public int ReservedStock { get; set; } = 0;

    /// <summary>樂觀並發控制版本戳記</summary>
    public byte[] RowVersion { get; set; } = [];

    /// <summary>是否上架（預設：true 上架中）</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>商品建立時間</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>商品最後更新時間</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>商品圖片集合（一對多）</summary>
    public ICollection<ProductImage> Images { get; set; } = [];
}
