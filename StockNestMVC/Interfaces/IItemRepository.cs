using StockNestMVC.Models;

namespace StockNestMVC.Interfaces;

public interface IItemRepository
{
    public Task CreateItem(Item item);
    public Task<IEnumerable<Item>> GetAll(int groupId, int categoryId);

    public Task<Item?> GetItemById(int categoryId, int itemIid);

    public Task UpdateItem(Item item);

    public Task<IEnumerable<Item>> DeleteItem(List<int> itemIds);

    public Task<bool> CheckDuplicateItem(int categoryId, string name, int? itemId);

}
