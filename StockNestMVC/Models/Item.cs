using System.ComponentModel.DataAnnotations;

namespace StockNestMVC.Models;

public class Item
{
    public int ItemId { get; set; }

    [Required]
    [MinLength(2, ErrorMessage = "Item name must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "Item name must not exceed 100 characters")]
    public string Name { get; set; }

    [Required]
    [Range(0, 9999)]
    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; }
}