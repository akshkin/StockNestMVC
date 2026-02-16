using System.ComponentModel.DataAnnotations;

namespace StockNestMVC.DTOs;

public class RegisterDto
{
    [Required]
    [MinLength(2, ErrorMessage = "First name must be atleast 2 characters")]
    [MaxLength(100, ErrorMessage = "First name cannot be more than 100 characters")]
    public string FirstName { get; set; }

    [Required]
    [MinLength(2, ErrorMessage = "Last name must be atleast 2 characters")]
    [MaxLength(100, ErrorMessage = "Last name cannot be more than 100 characters")]
    public string LastName { get; set; }

    [EmailAddress]
    public string EmailAddress { get; set; }

    public string Password { get; set; }
}
