using StockNestMVC.Models;

namespace StockNestMVC.Interfaces;

public interface IItemRepository
{
    public Task CreateItem(Item item);
    public Task<(IEnumerable<Item>, int total)> GetAll(int groupId, int categoryId, int page, int size);

    public Task<Item?> GetItemById(int categoryId, int itemIid);

    public Task UpdateItem(Item item);

    public Task<IEnumerable<Item>> DeleteItem(List<int> itemIds);

    public Task<bool> CheckDuplicateItem(int categoryId, string name, int? itemId);

}
