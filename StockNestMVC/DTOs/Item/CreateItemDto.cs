using System.ComponentModel.DataAnnotations;

namespace StockNestMVC.DTOs.Item;

public class CreateItemDto
{
    [Required]
    [MinLength(2, ErrorMessage = "Item name must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "Item name must not exceed 100 characters")]
    public string Name { get; set; }

    [Required]
    [Range(0, 9999)]
    public int Quantity { get; set; }
}
