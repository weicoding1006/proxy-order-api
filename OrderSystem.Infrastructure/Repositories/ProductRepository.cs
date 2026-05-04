using Microsoft.EntityFrameworkCore;
using OrderSystem.Application.Interfaces;
using OrderSystem.Domain.Entities;
using OrderSystem.Domain.Exceptions;
using OrderSystem.Infrastructure.Data;

namespace OrderSystem.Infrastructure.Repositories;

public class ProductRepository(OrderDbContext context) : IProductRepository
{
    public async Task<List<Product>> FindAllAsync()
        => await context.Products
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .ToListAsync();

    public async Task<Product?> FindByIdAsync(Guid id)
        => await context.Products
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Product> CreateAsync(Product product)
    {
        context.Products.Add(product);
        await context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        context.Products.Update(product);
        await context.SaveChangesAsync();
        return product;
    }

    public async Task DeleteAsync(Product product)
    {
        context.Products.Remove(product);
        await context.SaveChangesAsync();
    }

    public async Task<ProductImage> AddImageAsync(ProductImage image)
    {
        context.ProductImages.Add(image);
        await context.SaveChangesAsync();
        return image;
    }

    public async Task<ProductImage?> FindImageByIdAsync(Guid imageId)
        => await context.ProductImages.FindAsync(imageId);

    public async Task RemoveImageAsync(ProductImage image)
    {
        context.ProductImages.Remove(image);
        await context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
        => await context.SaveChangesAsync();

    public async Task UpdateReservedStockAsync(Guid productId, int delta)
    {
        var product = await context.Products.FindAsync(productId)
            ?? throw new ProductNotFoundException(productId);

        try
        {
            product.ReservedStock += delta;
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InsufficientStockException(product.Name, product.Stock - product.ReservedStock, 0);
        }
    }
}
