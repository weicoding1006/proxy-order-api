namespace OrderSystem.Application.DTOs.Order;

public class OrderItemResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Name { get; set; } = string.Empty;
}
