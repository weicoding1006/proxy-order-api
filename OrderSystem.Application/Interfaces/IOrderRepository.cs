using OrderSystem.Domain.Entities;

namespace OrderSystem.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order> CreateAsync(Order order);
    Task<List<Order>> FindByUserIdAsync(string userId);
    Task<Order?> FindByIdAsync(Guid id);
}
