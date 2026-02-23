using StockNestMVC.DTOs.Item;
using StockNestMVC.Models;

namespace StockNestMVC.Interfaces;

public interface IItemRepository
{
    public Task<IEnumerable<ItemDto>> GetAll(int groupId, int categoryId, AppUser user);
    public Task<ItemDto> CreateItem(int groupId, int categoryId, AppUser user, CreateItemDto createItemDto);

    public Task<ItemDto?> GetItemById(int groupId, int categoryId, int itemIid, AppUser user);

    public Task<ItemDto?> UpdateItem(int groupId, int categoryId, int itemId, AppUser user, CreateItemDto updateItemDto);

    public Task<IEnumerable<ItemDto?>> DeleteItem(int groupId, int categoryId, List<int> itemIds, AppUser user);

}
