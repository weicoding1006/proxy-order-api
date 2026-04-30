using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.DTOs;

public class ProductResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }
    public List<ProductImageDto> Images { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static ProductResponseDto FromEntity(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        Stock = product.Stock,
        IsActive = product.IsActive,
        Images = product.Images
            .OrderBy(i => i.SortOrder)
            .Select(ProductImageDto.FromEntity)
            .ToList(),
        CreatedAt = product.CreatedAt,
        UpdatedAt = product.UpdatedAt
    };
}
