using Microsoft.EntityFrameworkCore;
using StockNestMVC.Data;
using StockNestMVC.Interfaces;
using StockNestMVC.Models;

namespace StockNestMVC.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly ApplicationDbContext _context;

    public ItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateItem(Item item)
    {      
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Item>> GetAll(int groupId, int categoryId)
    {
        var items = await _context.Items.Where(i => i.CategoryId == categoryId).ToListAsync();

        return items;
    }

    public async Task<Item?> GetItemById(int categoryId, int itemIid)
    {
        var item = await _context.Items
            .FirstOrDefaultAsync(i => i.CategoryId == categoryId && i.ItemId == itemIid);
        return item;
    }

    public async Task UpdateItem(Item item)
    {
        _context.Items.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Item>> DeleteItem(List<int> itemIds)
    {
        var selectedItems = await _context.Items.Where(i => itemIds.Contains(i.ItemId)).ToListAsync();

        _context.Items.RemoveRange(selectedItems);
        await _context.SaveChangesAsync();
        return selectedItems;
    }

    public async Task<bool> CheckDuplicateItem(int categoryId, string name, int? itemId)
    {
        bool duplicate;

        if (itemId == null)
        {
            duplicate = await _context.Items
            .AnyAsync(i =>
                i.CategoryId == categoryId &&
                i.Name == name);
        }
        else
        {
            duplicate = await _context.Items
           .AnyAsync(i =>
               i.CategoryId == categoryId &&
               i.Name == name &&
               i.ItemId != itemId);
        }

        return duplicate;
    }
}
