using OrderSystem.Application.DTOs.Order;
using OrderSystem.Application.Interfaces;
using OrderSystem.Domain.Entities;
using OrderSystem.Domain.Exceptions;

namespace OrderSystem.Application.Services;

public class OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
{
    public async Task<OrderResponse> CreateAsync(CreateOrderRequest dto, string userId)
    {
        var orderItems = new List<OrderItem>();

        foreach (var item in dto.Items)
        {
            var product = await productRepository.FindByIdAsync(item.ProductId)
                ?? throw new ProductNotFoundException(item.ProductId);

            if (!product.IsActive)
                throw new InvalidProductDataException($"Product '{product.Name}' is not available.");

            if (product.Stock < item.Quantity)
                throw new InsufficientStockException(product.Name, product.Stock, item.Quantity);

            orderItems.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            TotalAmount = orderItems.Sum(i => i.UnitPrice * i.Quantity),
            OrderItems = orderItems
        };

        var created = await orderRepository.CreateAsync(order);
        return OrderResponse.FromEntity(created);
    }

    public async Task<List<OrderResponse>> FindByUserIdAsync(string userId)
    {
        var orders = await orderRepository.FindByUserIdAsync(userId);
        return orders.Select(OrderResponse.FromEntity).ToList();
    }

    public async Task<OrderResponse?> FindOneAsync(Guid id, string userId)
    {
        var order = await orderRepository.FindByIdAsync(id);
        if (order is null || order.UserId != userId)
            return null;
        return OrderResponse.FromEntity(order);
    }
}
