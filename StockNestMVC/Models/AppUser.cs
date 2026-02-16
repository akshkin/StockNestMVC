using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockNestMVC.Models;

public class AppUser : IdentityUser
{
    [Required]
    [MinLength(2, ErrorMessage = "First name must be atleast 2 characters")]
    [MaxLength(100, ErrorMessage = "First name cannot be more than 100 characters")]
    public string FirstName { get; set; }

    [Required]
    [MinLength(2, ErrorMessage = "Last name must be atleast 2 characters")]
    [MaxLength(100, ErrorMessage = "Last name cannot be more than 100 characters")]
    public string LastName { get; set; }

    [NotMapped]
    public string FullName => FirstName + LastName; 
    
    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
}
