namespace OrderSystem.Application.DTOs.Cart;

public class CartItemResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
}
