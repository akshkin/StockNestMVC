using StockNestMVC.Models;

namespace StockNestMVC.Interfaces;

public interface IUserRepository
{
    public Task Logout(AppUser user);
}
