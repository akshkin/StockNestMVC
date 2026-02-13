using System.ComponentModel.DataAnnotations;

namespace StockNestMVC.DTOs.Group;

public class CreateGroupDto
{
    [Required]
    [MinLength(2, ErrorMessage = "Group name must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "Group name must not exceed 100 characters")]
    public string Name { get; set; }
}
