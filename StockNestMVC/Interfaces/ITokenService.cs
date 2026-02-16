using StockNestMVC.Models;

namespace StockNestMVC.Interfaces;

public interface ITokenService
{
    public Task<string> CreateToken(AppUser user);
}
