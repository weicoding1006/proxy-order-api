using OrderSystem.Application.DTOs.Cart;
using OrderSystem.Application.DTOs.Order;

namespace OrderSystem.Application.Interfaces;

public interface ICartService
{
    Task<CartResponse> GetOrCreateCartAsync(string userId);
    Task<CartResponse> AddItemAsync(string userId, Guid productId, int quantity);
    Task<CartResponse> UpdateItemQuantityAsync(string userId, Guid cartItemId, int quantity);
    Task RemoveItemAsync(string userId, Guid cartItemId);
    Task ClearCartAsync(string userId);
    Task<OrderResponse> CheckoutAsync(string userId);
}
