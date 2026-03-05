using StockNestMVC.DTOs.Item;
using System.Security.Claims;

namespace StockNestMVC.Interfaces;

public interface IItemService
{
    public Task<ItemDto> CreateItem(int groupId, int categoryId, ClaimsPrincipal claimsPrincipal, CreateItemDto createItemDto);

    public Task<IEnumerable<ItemDto>> GetAll(int groupId, int categoryId, ClaimsPrincipal claimsPrincipal);

    public Task<ItemDto?> GetItemById(int groupId, int categoryId, int itemId, ClaimsPrincipal claimsPrincipal);

    public Task<ItemDto?> UpdateItem(int groupId, int categoryId, int itemId, ClaimsPrincipal claimsPrincipal, CreateItemDto updateItemDto);

    public Task<IEnumerable<ItemDto?>> DeleteItem(int groupId, int categoryId, List<int> itemIds, ClaimsPrincipal claimsPrincipal);
}
