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
                i.CategoryId == categoryId && 
                i.Name == createItemDto.Name);

        if (duplicate)
            throw new Exception("A category with this name already exists in the group");


        var item = new Item
        {
            Name = createItemDto.Name,
            Quantity = createItemDto.Quantity,
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

    public async Task<ItemDto?> GetItemById(int groupId, int categoryId, int itemIid, AppUser user)
    {
        var category = await _categoryRepo.GetCategoryById(groupId, categoryId, user);

        if (category == null) throw new Exception("Category not found");

        var item = await _context.Items
            .FirstOrDefaultAsync(i => i.CategoryId == categoryId && i.ItemId == itemIid);

        if (item == null) throw new Exception($"Item with id {itemIid} not found");

        return item.ToItemDto();
    }

    public async Task<ItemDto?> UpdateItem(int groupId, int categoryId, int itemId, AppUser user, CreateItemDto updateItemDto)
    {
        var category = await _categoryRepo.GetCategoryById(groupId, categoryId, user);

        if (category == null) throw new Exception("Category not found");

        var existingItem = await _context.Items
            .FirstOrDefaultAsync(i => i.CategoryId == categoryId && i.ItemId == itemId);

        if (existingItem == null) throw new Exception($"Item with id {itemId} not found");

        var userRole = await _groupRepo.GetRoleInGroup(groupId, user);

        if (userRole == "Viewer") throw new Exception("You do not have the permission to edit items");

        var duplicate = await _context.Items
            .AnyAsync(i =>
                i.CategoryId == categoryId &&
                i.Name == updateItemDto.Name && 
                i.ItemId != itemId);

        if (duplicate)
            throw new Exception("An item with this name already exists in the group");

        existingItem.Name = updateItemDto.Name;
        existingItem.Quantity = updateItemDto.Quantity;

        await _context.SaveChangesAsync();
        return existingItem.ToItemDto();
    }

    public async Task<IEnumerable<ItemDto?>> DeleteItem(int groupId, int categoryId, List<int> itemIds, AppUser user)
    {
        var category = await _categoryRepo.GetCategoryById(groupId, categoryId, user);

        if (category == null) throw new Exception("Category not found");


        var selectedItems = await _context.Items.Where(i => itemIds.Contains(i.ItemId)).ToListAsync();

        var userRole = await _groupRepo.GetRoleInGroup(groupId, user);

        if (userRole == "Viewer") throw new Exception("You do not have the permission to delete items");

        _context.Items.RemoveRange(selectedItems);
        await _context.SaveChangesAsync();

        return selectedItems.Select(i => i.ToItemDto());
    }
}
