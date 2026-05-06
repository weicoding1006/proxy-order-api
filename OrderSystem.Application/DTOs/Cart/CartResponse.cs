namespace OrderSystem.Application.DTOs.Cart;

public class CartResponse
{
    public Guid CartId { get; set; }
    public List<CartItemResponse> Items { get; set; } = [];
    public decimal TotalAmount { get; set; }
}
