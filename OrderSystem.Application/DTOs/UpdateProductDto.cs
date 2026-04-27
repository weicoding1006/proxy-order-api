using System.ComponentModel.DataAnnotations;

namespace OrderSystem.Application.DTOs;

public class UpdateProductDto
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal? Price { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public int? Stock { get; set; }

    public bool? IsActive { get; set; }
}
