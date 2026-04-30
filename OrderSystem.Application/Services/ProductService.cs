using Microsoft.AspNetCore.Http;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Interfaces;
using OrderSystem.Domain.Entities;
using OrderSystem.Domain.Exceptions;

namespace OrderSystem.Application.Services;

public class ProductService(IProductRepository repository, IFileStorageService fileStorage)
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

    public async Task<ProductImageDto> UploadImageAsync(Guid productId, IFormFile file)
    {
        if (file.Length > 5 * 1024 * 1024)
            throw new InvalidOperationException("Image file size must not exceed 5MB.");

        var product = await repository.FindByIdAsync(productId)
            ?? throw new ProductNotFoundException(productId);

        var folder = $"uploads/products/{productId}";
        var imageUrl = await fileStorage.SaveImageAsync(file, folder);

        var isCover = !product.Images.Any();
        var sortOrder = product.Images.Any() ? product.Images.Max(i => i.SortOrder) + 1 : 0;

        var image = new ProductImage
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            ImageUrl = imageUrl,
            IsCover = isCover,
            SortOrder = sortOrder,
            CreatedAt = DateTime.UtcNow
        };

        var created = await repository.AddImageAsync(image);
        return ProductImageDto.FromEntity(created);
    }

    public async Task RemoveImageAsync(Guid productId, Guid imageId)
    {
        var image = await repository.FindImageByIdAsync(imageId)
            ?? throw new ProductImageNotFoundException(imageId);

        if (image.ProductId != productId)
            throw new ProductImageNotFoundException(imageId);

        var wasCover = image.IsCover;
        fileStorage.DeleteImage(image.ImageUrl);
        await repository.RemoveImageAsync(image);

        if (wasCover)
        {
            var product = await repository.FindByIdAsync(productId);
            var next = product?.Images.MinBy(i => i.SortOrder);
            if (next is not null)
            {
                next.IsCover = true;
                await repository.SaveChangesAsync();
            }
        }
    }

    public async Task<ProductImageDto> SetCoverImageAsync(Guid productId, Guid imageId)
    {
        var product = await repository.FindByIdAsync(productId)
            ?? throw new ProductNotFoundException(productId);

        var target = product.Images.FirstOrDefault(i => i.Id == imageId)
            ?? throw new ProductImageNotFoundException(imageId);

        foreach (var img in product.Images)
            img.IsCover = img.Id == imageId;

        await repository.SaveChangesAsync();
        return ProductImageDto.FromEntity(target);
    }
}
