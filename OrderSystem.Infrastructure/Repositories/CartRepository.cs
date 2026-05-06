using Microsoft.EntityFrameworkCore;
using OrderSystem.Application.Interfaces;
using OrderSystem.Domain.Entities;
using OrderSystem.Infrastructure.Data;

namespace OrderSystem.Infrastructure.Repositories;

public class CartRepository(OrderDbContext context) : ICartRepository
{
    public Task<Cart?> GetByUserIdAsync(string userId)
        => context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

    public async Task<Cart> AddAsync(Cart cart)
    {
        context.Carts.Add(cart);
        await context.SaveChangesAsync();
        return cart;
    }

    public async Task<CartItem> AddCartItemAsync(CartItem item)
    {
        context.CartItems.Add(item);
        await context.SaveChangesAsync();
        return item;
    }

    public Task SaveChangesAsync()
        => context.SaveChangesAsync();
}
