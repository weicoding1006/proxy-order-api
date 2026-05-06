using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(string userId);
    Task<Cart> AddAsync(Cart cart);
    Task<CartItem> AddCartItemAsync(CartItem item);
    Task SaveChangesAsync();
}
