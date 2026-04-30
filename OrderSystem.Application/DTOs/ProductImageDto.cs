using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.DTOs;

public class ProductImageDto
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsCover { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }

    public static ProductImageDto FromEntity(ProductImage image) => new()
    {
        Id = image.Id,
        ImageUrl = image.ImageUrl,
        IsCover = image.IsCover,
        SortOrder = image.SortOrder,
        CreatedAt = image.CreatedAt
    };
}
