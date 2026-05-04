using OrderSystem.Domain.Enums;

namespace OrderSystem.Application.DTOs.Order;

public class UpdateOrderStatusRequest
{
    public OrderStatus Status { get; set; }
}
