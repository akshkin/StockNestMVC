using StockNestMVC.DTOs.Stats;
using System.Security.Claims;

namespace StockNestMVC.Interfaces;

public interface IStatsService
{
    public Task<StatsDto> GetGroupStats(ClaimsPrincipal principal);
}
