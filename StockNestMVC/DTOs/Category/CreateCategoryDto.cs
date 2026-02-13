using System.ComponentModel.DataAnnotations;

namespace StockNestMVC.DTOs.Category;

public class CreateCategoryDto
{
    [Required]
    [MinLength(2, ErrorMessage = "Category name must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "Category name must not exceed 100 characters")]
    public string Name { get; set; }
}
