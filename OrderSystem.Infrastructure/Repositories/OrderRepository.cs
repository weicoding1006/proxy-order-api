using Microsoft.EntityFrameworkCore;
using OrderSystem.Application.Interfaces;
using OrderSystem.Domain.Entities;
using OrderSystem.Domain.Enums;
using OrderSystem.Domain.Exceptions;
using OrderSystem.Infrastructure.Data;

namespace OrderSystem.Infrastructure.Repositories;

public class OrderRepository(OrderDbContext context) : IOrderRepository
{
    public async Task<Order> CreateAsync(Order order)
    {
        context.Orders.Add(order);
        await context.SaveChangesAsync();
        return order;
    }

    public async Task<List<Order>> FindByUserIdAsync(string userId)
        => await context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .Where(o => o.UserId == userId)
            .ToListAsync();

    public async Task<Order?> FindByIdAsync(Guid id)
        => await context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

    public Task<List<Order>> FindAllAsync()
        => context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .ToListAsync();

    public async Task<Order> UpdateStatusAsync(Guid orderId, OrderStatus newStatus)
    {
        var order = await context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new OrderNotFoundException(orderId);

        order.Status = newStatus;
        await context.SaveChangesAsync();
        return order;
    }
}
