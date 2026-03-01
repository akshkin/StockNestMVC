using Microsoft.EntityFrameworkCore;
using StockNestMVC.Data;
using StockNestMVC.DTOs.Stats;
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

    public async Task<int> CountGroups(AppUser user)
    {
        return _context.UserGroup.Where(ug => ug.UserId == user.Id).Count();
    }

    public async Task<int> CountCategories(AppUser user)
    {
        return  _context.Categories
            .Where(c => c.Group.UserGroups.Any(ug => ug.UserId == user.Id))
            .Count();
    }

    public async Task<int> CountItems(AppUser user)
    {
        return _context.Items
            .Where(i => i.Category.Group.UserGroups.Any(ug => ug.UserId == user.Id))
            .Count();
    }

    public async Task<int> CountItemsCreatedByUser(AppUser user)
    {
        return _context.Items.Where(i => i.CreatedBy == user.Id).Count();
    }

    public async Task<int> CountItemsUpdatedByUser(AppUser user)
    {
        return _context.Items.Where(i => i.UpdatedBy == user.Id).Count();
    }

    public async Task<List<ItemsPerCategoryDto>> GetItemsPerGroup(AppUser user)
    {
        return await _context.Items
            .Where(i => i.Category.Group.UserGroups.Any(ug => ug.UserId == user.Id))
            .GroupBy(i => new 
            {
                CategoryId = i.Category.CategoryId,
                CategoryName = i.Category.Name,
                GroupId = i.Category.Group.GroupId,
                GroupName = i.Category.Group.Name
            })
            .Select(g => new ItemsPerCategoryDto
            {
                GroupName = g.Key.GroupName,
                GroupId = g.Key.GroupId,
                CategoryName =  g.Key.CategoryName,
                CategoryId = g.Key.CategoryId,
                Count = g.Count()
            })
            .ToListAsync();
    }

    public async Task<List<ItemsPerCategoryDto>> GetTopCategories(AppUser user)
    {
        var itemsPerCategory = await GetItemsPerGroup(user);
        var topCategories = itemsPerCategory.OrderByDescending(i => i.Count).Take(3).ToList();
        return topCategories;
    }
}
