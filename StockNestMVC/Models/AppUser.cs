using Microsoft.AspNetCore.Identity;

namespace StockNestMVC.Models;

public class AppUser : IdentityUser
{
    public string FullName { get; set; }
}
