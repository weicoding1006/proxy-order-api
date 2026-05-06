using System.ComponentModel.DataAnnotations;

namespace OrderSystem.Application.DTOs.Cart;

public class AddCartItemRequest
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }
}
