using OrderSystem.Application.DTOs;
using OrderSystem.Application.Interfaces;
using OrderSystem.Domain.Entities;
using OrderSystem.Domain.Exceptions;

namespace OrderSystem.Application.Services;

public class ProductService(IProductRepository repository)
{
    public async Task<List<ProductResponseDto>> FindAllAsync()
    {
        var products = await repository.FindAllAsync();
        return products.Select(ProductResponseDto.FromEntity).ToList();
    }

    public async Task<ProductResponseDto> FindOneAsync(Guid id)
    {
        var product = await repository.FindByIdAsync(id)
            ?? throw new ProductNotFoundException(id);
        return ProductResponseDto.FromEntity(product);
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
    {
        if (dto.Price <= 0)
            throw new InvalidProductDataException("Price must be greater than 0.");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description ?? string.Empty,
            Price = dto.Price,
            Stock = dto.Stock,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await repository.CreateAsync(product);
        return ProductResponseDto.FromEntity(created);
    }

    public async Task<ProductResponseDto> UpdateAsync(Guid id, UpdateProductDto dto)
    {
        var product = await repository.FindByIdAsync(id)
            ?? throw new ProductNotFoundException(id);

        if (dto.Price.HasValue && dto.Price.Value <= 0)
            throw new InvalidProductDataException("Price must be greater than 0.");

        if (dto.Name is not null) product.Name = dto.Name;
        if (dto.Description is not null) product.Description = dto.Description;
        if (dto.Price.HasValue) product.Price = dto.Price.Value;
        if (dto.Stock.HasValue) product.Stock = dto.Stock.Value;
        if (dto.IsActive.HasValue) product.IsActive = dto.IsActive.Value;

        var updated = await repository.UpdateAsync(product);
        return ProductResponseDto.FromEntity(updated);
    }

    public async Task RemoveAsync(Guid id)
    {
        var product = await repository.FindByIdAsync(id)
            ?? throw new ProductNotFoundException(id);
        await repository.DeleteAsync(product);
    }
}
