using StockNestMVC.Models;

namespace StockNestMVC.Interfaces;

public interface IUserRepository
{
    public Task SaveRefreshToken(AppUser user, string newRefreshToken);

    public Task Logout(AppUser user);
}
