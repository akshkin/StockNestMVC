using StockNestMVC.DTOs.Item;
using StockNestMVC.Models;

namespace StockNestMVC.Interfaces;

public interface IItemRepository
{
    public Task<ItemDto> CreateItem(int groupId, int categoryId, AppUser user, CreateItemDto createItemDto);
}
