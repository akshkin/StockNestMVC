using System.ComponentModel.DataAnnotations;

namespace StockNestMVC.Models;

public class Group
{
    public int GroupId { get; set; }

    [Required]
    [MinLength(2, ErrorMessage = "Group name must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "Group name must not exceed 100 characters")]
    public string Name { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // users in this group with their roles
    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();

    public ICollection<Category> Categories { get; set; } = new List<Category>();

}
