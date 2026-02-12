using System.ComponentModel.DataAnnotations;

namespace StockNestMVC.Models;

public class Category
{
    public int CategoryId { get; set; }

    [Required]
    [MinLength(2, ErrorMessage = "Category name must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "Category name must not exceed 100 characters")]
    public string Name { get; set; }

    public int GroupId { get; set; }
    public Group Group { get; set; }

    public ICollection<Item> Items { get; set; } = new List<Item>();
}