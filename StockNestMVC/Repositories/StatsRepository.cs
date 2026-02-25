using Microsoft.EntityFrameworkCore;
using StockNestMVC.Data;
using StockNestMVC.Interfaces;
using StockNestMVC.Models;

namespace StockNestMVC.Repositories;

public class StatsRepository : IStatsRepository
{
    private readonly ApplicationDbContext _context;

    public StatsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task GetStats(AppUser user)
    {
        
    }
}
