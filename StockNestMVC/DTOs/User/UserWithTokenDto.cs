using StockNestMVC.Models;

namespace StockNestMVC.DTOs.User;

public class UserWithTokenDto
{
    public UserDto User { get; set; }
    public string Token { get; set; }
}
