using StockNestMVC.Models;

namespace StockNestMVC.DTOs;

public class UserWithTokenDto
{
    public UserDto User { get; set; }
    public string Token { get; set; }
}
