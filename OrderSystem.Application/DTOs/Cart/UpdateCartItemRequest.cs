using System.ComponentModel.DataAnnotations;

namespace OrderSystem.Application.DTOs.Cart;

public class UpdateCartItemRequest
{
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be 0 or greater.")]
    public int Quantity { get; set; }
}
