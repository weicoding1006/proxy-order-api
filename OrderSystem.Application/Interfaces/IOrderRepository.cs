using OrderSystem.Domain.Entities;
using OrderSystem.Domain.Enums;

namespace OrderSystem.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);
    Task<List<Order>> FindByUserIdAsync(string userId);
    Task<Order?> FindByIdAsync(Guid id);
    Task<List<Order>> FindAllAsync();
    Task<Order> UpdateStatusAsync(Guid orderId, OrderStatus newStatus);
}
