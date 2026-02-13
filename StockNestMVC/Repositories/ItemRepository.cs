using Microsoft.EntityFrameworkCore;
using StockNestMVC.Data;
using StockNestMVC.DTOs.Item;
using StockNestMVC.Interfaces;
using StockNestMVC.Mappers;
using StockNestMVC.Models;

namespace StockNestMVC.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IGroupRepository _groupRepo;

    public ItemRepository(ApplicationDbContext context, ICategoryRepository categoryRepo, IGroupRepository groupRepo)
    {
        _context = context;
        _categoryRepo = categoryRepo;
        _groupRepo = groupRepo;
    }

    public async Task<ItemDto> CreateItem(int groupId, int categoryId, AppUser user, CreateItemDto createItemDto)
    {
        var category = await _categoryRepo.GetCategoryById(groupId, categoryId, user);
        if (category == null) throw new Exception("Category not found");

        var userRole = await _groupRepo.GetRoleInGroup(groupId, user);

        if (userRole == "Viewer") throw new Exception("You do not have the permission to create items");

        var duplicate = await _context.Items
            .AnyAsync(i => 
                i.GroupId == groupId && 
                i.CategoryId == categoryId && 
                i.Name == createItemDto.Name);

        if (duplicate)
            throw new Exception("A category with this name already exists in the group");


        var item = new Item
        {
            Name = createItemDto.Name,
            Quantity = createItemDto.Quantity,
            GroupId = groupId,
            CategoryId = categoryId,
        };
        
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();

        return item.ToItemDto();
    }

    public async Task<IEnumerable<ItemDto>> GetAll(int groupId, int categoryId, AppUser user)
    {
        var category = await _categoryRepo.GetCategoryById(groupId, categoryId, user);

        if (category == null) throw new Exception("Category not found");

        var items = await _context.Items.Where(i => i.CategoryId == categoryId).ToListAsync();

        return items.Select(i => i.ToItemDto());
    }
}
