using StockNestMVC.DTOs.Stats;
using StockNestMVC.Models;

namespace StockNestMVC.Interfaces;

public interface IStatsRepository
{
    Task<int> CountGroups(AppUser user);
    Task<int> CountCategories(AppUser user);
    Task<int> CountItems(AppUser user);
    Task<int> CountItemsCreatedByUser(AppUser user);
    Task<int> CountItemsUpdatedByUser(AppUser user);

    Task<List<ItemsPerCategoryDto>> GetTopCategories(AppUser user); 
    Task<List<ItemsPerCategoryDto>> GetItemsPerGroup(AppUser user);

}
