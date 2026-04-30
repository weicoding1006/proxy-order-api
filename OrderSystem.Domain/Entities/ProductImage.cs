namespace OrderSystem.Domain.Entities;

public class ProductImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsCover { get; set; } = false;
    public int SortOrder { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Product Product { get; set; } = null!;
}
