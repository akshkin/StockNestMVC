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

        Console.WriteLine("userRole", userRole);

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
}
