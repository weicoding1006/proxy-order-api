using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> FindAllAsync();
    Task<Product?> FindByIdAsync(Guid id);
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(Product product);

    Task<ProductImage> AddImageAsync(ProductImage image);
    Task<ProductImage?> FindImageByIdAsync(Guid imageId);
    Task RemoveImageAsync(ProductImage image);
    Task SaveChangesAsync();
}
