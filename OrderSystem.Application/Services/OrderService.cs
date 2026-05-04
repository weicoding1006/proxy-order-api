using OrderSystem.Application.DTOs.Order;
using OrderSystem.Application.Interfaces;
using OrderSystem.Domain.Entities;
using OrderSystem.Domain.Enums;
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

            var available = product.Stock - product.ReservedStock;
            if (available < item.Quantity)
                throw new InsufficientStockException(product.Name, available, item.Quantity);

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
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            TotalAmount = orderItems.Sum(i => i.UnitPrice * i.Quantity),
            OrderItems = orderItems
        };

        var created = await orderRepository.CreateAsync(order);

        // Reserve stock for each item after order is persisted
        foreach (var item in dto.Items)
            await productRepository.UpdateReservedStockAsync(item.ProductId, item.Quantity);

        return OrderResponse.FromEntity(created);
    }

    public async Task<OrderResponse> UpdateStatusAsync(
        Guid orderId, OrderStatus newStatus, string callerId, bool isAdmin)
    {
        var order = await orderRepository.FindByIdAsync(orderId)
            ?? throw new OrderNotFoundException(orderId);

        ValidateTransition(order, newStatus, callerId, isAdmin);

        var previousStatus = order.Status;
        var updated = await orderRepository.UpdateStatusAsync(orderId, newStatus);

        await AdjustInventoryOnTransitionAsync(updated, previousStatus, newStatus);

        return OrderResponse.FromEntity(updated);
    }

    private static void ValidateTransition(Order order, OrderStatus to, string callerId, bool isAdmin)
    {
        var from = order.Status;

        bool allowed = (from, to) switch
        {
            (OrderStatus.Pending, OrderStatus.Confirmed) => isAdmin,
            (OrderStatus.Pending, OrderStatus.Cancelled) => isAdmin || order.UserId == callerId,
            (OrderStatus.Confirmed, OrderStatus.Shipped) => isAdmin,
            (OrderStatus.Confirmed, OrderStatus.Cancelled) => isAdmin,
            (OrderStatus.Shipped, OrderStatus.Completed) => isAdmin,
            _ => false
        };

        if (!allowed)
        {
            // Distinguish: transition exists but caller lacks privilege vs. transition is simply invalid
            bool transitionExists = (from, to) switch
            {
                (OrderStatus.Pending, OrderStatus.Confirmed) => true,
                (OrderStatus.Pending, OrderStatus.Cancelled) => true,
                (OrderStatus.Confirmed, OrderStatus.Shipped) => true,
                (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,
                (OrderStatus.Shipped, OrderStatus.Completed) => true,
                _ => false
            };

            if (transitionExists)
                throw new UnauthorizedAccessException(
                    $"You do not have permission to transition order from '{from}' to '{to}'.");

            throw new InvalidOrderStatusTransitionException(from, to);
        }
    }

    private async Task AdjustInventoryOnTransitionAsync(
        Order order, OrderStatus from, OrderStatus to)
    {
        // Pending → Confirmed: release reservation, formally deduct stock
        if (from == OrderStatus.Pending && to == OrderStatus.Confirmed)
        {
            foreach (var item in order.OrderItems)
            {
                await productRepository.UpdateReservedStockAsync(item.ProductId, -item.Quantity);
                var product = await productRepository.FindByIdAsync(item.ProductId)
                    ?? throw new ProductNotFoundException(item.ProductId);
                product.Stock -= item.Quantity;
                await productRepository.UpdateAsync(product);
            }
            return;
        }

        // Pending → Cancelled: return reservation
        if (from == OrderStatus.Pending && to == OrderStatus.Cancelled)
        {
            foreach (var item in order.OrderItems)
                await productRepository.UpdateReservedStockAsync(item.ProductId, -item.Quantity);
            return;
        }

        // Confirmed → Cancelled: return formally deducted stock
        if (from == OrderStatus.Confirmed && to == OrderStatus.Cancelled)
        {
            foreach (var item in order.OrderItems)
            {
                var product = await productRepository.FindByIdAsync(item.ProductId)
                    ?? throw new ProductNotFoundException(item.ProductId);
                product.Stock += item.Quantity;
                await productRepository.UpdateAsync(product);
            }
        }
    }

    public async Task<List<OrderResponse>> FindByUserIdAsync(string userId)
    {
        var orders = await orderRepository.FindByUserIdAsync(userId);
        return orders.Select(OrderResponse.FromEntity).ToList();
    }

    public async Task<List<OrderResponse>> FindAllAsync()
    {
        var orders = await orderRepository.FindAllAsync();
        return orders.Select(OrderResponse.FromEntity).ToList();
    }

    public async Task<OrderResponse?> FindOneAsync(Guid id)
    {
        var order = await orderRepository.FindByIdAsync(id);
        if (order is null)
            return null;
        return OrderResponse.FromEntity(order);
    }
}
