using OrderSystem.Application.DTOs.Cart;
using OrderSystem.Application.DTOs.Order;
using OrderSystem.Application.Interfaces;
using OrderSystem.Domain.Entities;
using OrderSystem.Domain.Exceptions;

namespace OrderSystem.Application.Services;

public class CartService(
    ICartRepository cartRepository,
    IProductRepository productRepository,
    OrderService orderService) : ICartService
{
    public async Task<CartResponse> GetOrCreateCartAsync(string userId)
    {
        var cart = await cartRepository.GetByUserIdAsync(userId);
        if (cart is null)
        {
            cart = new Cart { Id = Guid.NewGuid(), UserId = userId };
            await cartRepository.AddAsync(cart);
        }
        return ToResponse(cart);
    }

    public async Task<CartResponse> AddItemAsync(string userId, Guid productId, int quantity)
    {
        var product = await productRepository.FindByIdAsync(productId)
            ?? throw new ProductNotFoundException(productId);

        if (!product.IsActive)
            throw new InvalidProductDataException($"Product '{product.Name}' is not available.");

        var cart = await cartRepository.GetByUserIdAsync(userId);
        if (cart is null)
        {
            cart = new Cart { Id = Guid.NewGuid(), UserId = userId };
            await cartRepository.AddAsync(cart);
            // Re-fetch with includes
            cart = (await cartRepository.GetByUserIdAsync(userId))!;
        }

        var existing = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
        if (existing is not null)
        {
            existing.Quantity += quantity;
            cart.UpdatedAt = DateTime.UtcNow;
            await cartRepository.SaveChangesAsync();
        }
        else
        {
            await cartRepository.AddCartItemAsync(new CartItem
            {
                CartId = cart.Id,
                ProductId = productId,
                Quantity = quantity
            });
            cart.UpdatedAt = DateTime.UtcNow;
            await cartRepository.SaveChangesAsync();
        }

        // Re-fetch to populate Product navigation on new items
        cart = (await cartRepository.GetByUserIdAsync(userId))!;
        return ToResponse(cart);
    }

    public async Task<CartResponse> UpdateItemQuantityAsync(string userId, Guid cartItemId, int quantity)
    {
        var cart = await cartRepository.GetByUserIdAsync(userId)
            ?? throw new CartEmptyException();

        var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId)
            ?? throw new KeyNotFoundException($"CartItem '{cartItemId}' not found.");

        if (quantity == 0)
            cart.CartItems.Remove(item);
        else
            item.Quantity = quantity;

        cart.UpdatedAt = DateTime.UtcNow;
        await cartRepository.SaveChangesAsync();

        cart = (await cartRepository.GetByUserIdAsync(userId))!;
        return ToResponse(cart);
    }

    public async Task RemoveItemAsync(string userId, Guid cartItemId)
    {
        var cart = await cartRepository.GetByUserIdAsync(userId)
            ?? throw new KeyNotFoundException($"CartItem '{cartItemId}' not found.");

        var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId)
            ?? throw new KeyNotFoundException($"CartItem '{cartItemId}' not found.");

        cart.CartItems.Remove(item);
        cart.UpdatedAt = DateTime.UtcNow;
        await cartRepository.SaveChangesAsync();
    }

    public async Task ClearCartAsync(string userId)
    {
        var cart = await cartRepository.GetByUserIdAsync(userId);
        if (cart is null) return;

        cart.CartItems.Clear();
        cart.UpdatedAt = DateTime.UtcNow;
        await cartRepository.SaveChangesAsync();
    }

    public async Task<OrderResponse> CheckoutAsync(string userId)
    {
        var cart = await cartRepository.GetByUserIdAsync(userId);
        if (cart is null || !cart.CartItems.Any())
            throw new CartEmptyException();

        var request = new CreateOrderRequest
        {
            Items = cart.CartItems.Select(ci => new OrderItemRequest
            {
                ProductId = ci.ProductId,
                Quantity = ci.Quantity
            }).ToList()
        };

        var order = await orderService.CreateAsync(request, userId);

        cart.CartItems.Clear();
        cart.UpdatedAt = DateTime.UtcNow;
        await cartRepository.SaveChangesAsync();

        return order;
    }

    private static CartResponse ToResponse(Cart cart) => new()
    {
        CartId = cart.Id,
        Items = cart.CartItems.Select(ci => new CartItemResponse
        {
            Id = ci.Id,
            ProductId = ci.ProductId,
            ProductName = ci.Product?.Name ?? string.Empty,
            CurrentPrice = ci.Product?.Price ?? 0,
            Quantity = ci.Quantity,
            Subtotal = (ci.Product?.Price ?? 0) * ci.Quantity
        }).ToList(),
        TotalAmount = cart.CartItems.Sum(ci => (ci.Product?.Price ?? 0) * ci.Quantity)
    };
}
