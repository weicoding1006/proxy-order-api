namespace OrderSystem.Domain.Entities;

public class ProductImage
{
    /// <summary>圖片唯一識別碼</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>所屬商品的 ID（外鍵）</summary>
    public Guid ProductId { get; set; }

    /// <summary>圖片網址</summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>是否為封面圖（主圖），商品列表與詳情頁的代表性圖片</summary>
    public bool IsCover { get; set; } = false;

    /// <summary>圖片排列順序（數字越小越前面）</summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>圖片上傳時間</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>導覽屬性：所屬商品</summary>
    public Product Product { get; set; } = null!;
}
