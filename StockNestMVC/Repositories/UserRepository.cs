using Microsoft.EntityFrameworkCore;
using StockNestMVC.Data;
using StockNestMVC.Interfaces;
using StockNestMVC.Models;

namespace StockNestMVC.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SaveRefreshToken(AppUser user, string newRefreshToken)
    {
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(1);
        //_context.Users.Update();
        await _context.SaveChangesAsync();
    }

    public async Task Logout(AppUser user)
    {
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.MinValue;
        await _context.SaveChangesAsync();
    }
}
