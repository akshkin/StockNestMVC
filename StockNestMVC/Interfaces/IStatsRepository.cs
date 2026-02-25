using StockNestMVC.Models;

namespace StockNestMVC.Interfaces;

public interface IStatsRepository
{
    public Task GetStats(AppUser user);
}
