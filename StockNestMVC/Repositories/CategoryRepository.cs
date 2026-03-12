using Microsoft.EntityFrameworkCore;
using StockNestMVC.Data;
using StockNestMVC.DTOs;
using StockNestMVC.Interfaces;
using StockNestMVC.Models;

namespace StockNestMVC.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateCategory(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Category>> GetCategoriesInGroup(int groupId)
    {
        var categories = await _context.Categories.Where(c => c.GroupId == groupId).ToListAsync();

        return categories;
    }

    public async Task<Category?> GetCategoryById(int groupId, int categoryId)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.GroupId == groupId);

        return category;
    }

    public async Task UpdateCategory(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCategory(Category category)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CheckDuplicate(int groupId, string name, int? categoryId)
    {

        bool duplicate;

        if(categoryId != null) // for update, allow duplicate if user keeps the same name for the given category
        {
            duplicate = await _context.Categories
           .AnyAsync(c =>
               c.GroupId == groupId &&
               c.Name.ToLower() == name.ToLower() &&
               c.CategoryId != categoryId);
        } 
        else // for create 
        {
            duplicate = await _context.Categories
           .AnyAsync(c => c.GroupId == groupId && c.Name.ToLower() == name.ToLower());
        }
        return duplicate;
    }

    public async Task<IEnumerable<SearchResultDto>> GetSearchResult(AppUser user, string searchTerm)
    {
        var categories = await _context.Categories
            .Where(c => c.Group.UserGroups.Any(ug => ug.UserId == user.Id) && c.Name.ToLower().Contains(searchTerm.Trim().ToLower()))
            .Select(c => new SearchResultDto
            {
                Type = "Category",
                GroupId = c.GroupId,
                Name = c.Name,
                CategoryId = c.CategoryId
            })
            .ToListAsync();

        return categories;        
    }


}
