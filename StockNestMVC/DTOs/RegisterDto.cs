using System.ComponentModel.DataAnnotations;

namespace StockNestMVC.DTOs;

public class RegisterDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [EmailAddress]
    public string EmailAddress { get; set; }

    public string Password { get; set; }
}
