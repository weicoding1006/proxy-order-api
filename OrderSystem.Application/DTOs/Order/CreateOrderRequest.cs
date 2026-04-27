using System.ComponentModel.DataAnnotations;

namespace OrderSystem.Application.DTOs.Order;

public class CreateOrderRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "At least one order item is required.")]
    public List<OrderItemRequest> Items { get; set; } = [];
}
