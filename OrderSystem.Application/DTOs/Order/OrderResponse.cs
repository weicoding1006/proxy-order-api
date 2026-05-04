using OrderEntity = OrderSystem.Domain.Entities.Order;

namespace OrderSystem.Application.DTOs.Order;

public class OrderResponse
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<OrderItemResponse> Items { get; set; } = [];

    public static OrderResponse FromEntity(OrderEntity order) => new()
    {
        Id = order.Id,
        UserId = order.UserId,
        TotalAmount = order.TotalAmount,
        Status = order.Status.ToString(),
        CreatedAt = order.CreatedAt,
        Items = order.OrderItems.Select(i => new OrderItemResponse
        {
            Id = i.Id,
            ProductId = i.ProductId,
            Name = i.Product.Name,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
        }).ToList()
    };
}
