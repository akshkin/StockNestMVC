using Microsoft.EntityFrameworkCore;
using StockNestMVC.Data;
using StockNestMVC.DTOs.Category;
using StockNestMVC.Interfaces;
using StockNestMVC.Mappers;
using StockNestMVC.Models;

namespace StockNestMVC.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IGroupRepository _groupRepo;

    public CategoryRepository(ApplicationDbContext context, IGroupRepository groupRepo)
    {
        _context = context;
        _groupRepo = groupRepo;
    }

    public async Task<CategoryDto> CreateCategory(int groupId, AppUser user, CreateCategoryDto createCategoryDto)
    {
        var userRole = await _groupRepo.GetRoleInGroup(groupId, user);

        if (userRole != "Owner") throw new Exception("Only owners can create categories");

        var duplicate = await _context.Categories
            .AnyAsync(c => c.GroupId == groupId && c.Name == createCategoryDto.Name);

        if (duplicate)
            throw new Exception("A category with this name already exists in the group");

        var newCategory = new Category 
        { 
            Name = createCategoryDto.Name,
            GroupId = groupId
        };

        _context.Categories.Add(newCategory);
        await _context.SaveChangesAsync();

        return newCategory.ToCategoryDto();
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesInGroup(int groupId, AppUser user)
    {
        //  Check if user belongs to the group
        var membership = await _context.UserGroup
            .FirstOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == user.Id);

        if (membership == null)
            throw new Exception("You are not a member of this group");

        var categories = await _context.Categories.Where(c => c.GroupId == groupId).ToListAsync();

        return categories.Select(c => c.ToCategoryDto());
    }

    public async Task<CategoryDto?> GetCategoryById(int groupId, int categoryId, AppUser user)
    {
        var membership = await _context.UserGroup
           .FirstOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == user.Id);

        if (membership == null)
            throw new Exception("You are not a member of this group");

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.GroupId == groupId);

        if (category == null) return null;

        return category.ToCategoryDto();
    }

    public async Task<CategoryDto?> UpdateCategory(int groupId, int categoryId, AppUser user, CreateCategoryDto updateCategoryDto)
    {
        var category = await GetCategoryById(groupId, categoryId, user);

        if (category == null) return null;

        var userRole = await _groupRepo.GetRoleInGroup(groupId, user);

        if (userRole == "Viewer") throw new Exception("You do not have permission to update categories");

        var duplicate = await _context.Categories
           .AnyAsync(c => 
           c.GroupId == groupId && 
           c.Name == updateCategoryDto.Name && 
           c.CategoryId != categoryId);

        if (duplicate)
            throw new Exception("A category with this name already exists in the group");

        category.Name = updateCategoryDto.Name;
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<CategoryDto?> DeleteCategory(int groupId, int categoryId, AppUser user)
    {
        var membership = await _context.UserGroup
           .FirstOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == user.Id);

        if (membership == null)
            throw new Exception("You are not a member of this group");

        if (membership.Role != "Owner")
            throw new Exception("Only owners can delete a category");

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.GroupId == groupId);

        if (category == null) return null;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return category.ToCategoryDto();
    }
}
