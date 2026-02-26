using Microsoft.AspNetCore.Identity;
using StockNestMVC.DTOs.Item;
using StockNestMVC.Interfaces;
using StockNestMVC.Mappers;
using StockNestMVC.Models;
using System.Security.Claims;

namespace StockNestMVC.Services;

public class ItemService : IItemService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IItemRepository _itemRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly UserGroupService _userGroupService;

    public ItemService(UserManager<AppUser> userManager, IItemRepository itemRepo, ICategoryRepository categoryRepo, UserGroupService userGroupService)
    {
        _userManager = userManager;
        _itemRepo = itemRepo;
        _categoryRepo = categoryRepo;
        _userGroupService = userGroupService;
    }

    public async Task<ItemDto> CreateItem(int groupId, int categoryId, ClaimsPrincipal claimsPrincipal, CreateItemDto createItemDto)
    {
        var (user, membership) = await _userGroupService.ValidateMutationOperations(claimsPrincipal, groupId);
    
        var duplicate = await _itemRepo.CheckDuplicateItem(categoryId, createItemDto.Name, null);

        if (duplicate) throw new Exception("An item with the same name already exists in the category");

        var item = new Item
        {
            Name = createItemDto.Name,
            Quantity = createItemDto.Quantity,
            CategoryId = categoryId,
            CreatedBy = user.Id
        };

        await _itemRepo.CreateItem(item);

        return item.ToItemDto(user.FullName, null);
    }

    public async Task<IEnumerable<ItemDto>> GetAll(int groupId, int categoryId, ClaimsPrincipal claimsPrincipal)
    {
        var (user, membership) = await _userGroupService.ValidateMembership(claimsPrincipal, groupId);

        var category = await _categoryRepo.GetCategoryById(groupId, categoryId);

        if (category == null) throw new Exception("Category not found");

        var items = await _itemRepo.GetAll(groupId, categoryId);

        var itemsDto = new List<ItemDto>();

        foreach(var item in items)
        {
            var (creator, updator) = await GetCreatorUpdatorNames(item, user);
            itemsDto.Add(item.ToItemDto(creator, updator));
        }
        return itemsDto;
    }

    public async Task<ItemDto?> GetItemById(int groupId, int categoryId, int itemId, ClaimsPrincipal claimsPrincipal)
    {
        var (user, membership) = await _userGroupService.ValidateMembership(claimsPrincipal, groupId);

        var category = await _categoryRepo.GetCategoryById(groupId, categoryId);

        if (category == null) throw new Exception("Category not found");

        var item = await _itemRepo.GetItemById(categoryId, itemId);

        if (item == null) throw new Exception("Item was not found");

        var (creator, updator) = await GetCreatorUpdatorNames(item, user);

        return item.ToItemDto(creator, updator);
    }

    public async Task<ItemDto?> UpdateItem(int groupId, int categoryId, int itemId, ClaimsPrincipal claimsPrincipal, CreateItemDto updateItemDto)
    {
        var (user, membership) = await _userGroupService.ValidateMutationOperations(claimsPrincipal, groupId);

        var category = await _categoryRepo.GetCategoryById(groupId, categoryId);

        if (category == null) throw new Exception("Category not found");

        var item = await _itemRepo.GetItemById(categoryId, itemId);

        if (item == null) throw new Exception("Item was not found");

        var duplicate = await _itemRepo.CheckDuplicateItem(categoryId, updateItemDto.Name, itemId);

        if (duplicate) throw new Exception("Item with the same name already exists in this category");

        item.Name = updateItemDto.Name;
        item.Quantity = updateItemDto.Quantity;
        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = user.Id;

        await _itemRepo.UpdateItem(item);

        return item.ToItemDto(user.FullName, item.UpdatedBy);
    }

    public async Task<IEnumerable<ItemDto?>> DeleteItem(int groupId, int categoryId, List<int> itemIds, ClaimsPrincipal claimsPrincipal)
    {
        var (user, membership) = await _userGroupService.ValidateMutationOperations(claimsPrincipal, groupId);

        var category = await _categoryRepo.GetCategoryById(groupId, categoryId);

        if (category == null) throw new Exception("Category not found");

        var items = await _itemRepo.DeleteItem(itemIds);

        return items.Select(i => i.ToItemDto(i.CreatedBy, i.UpdatedBy));
    }

    private async Task<(string? createdBy, string? updatedBy)> GetCreatorUpdatorNames(Item item, AppUser currentUser)
    {
        string? creatorName = null;
        if (item.CreatedBy != null)
        {
            var creator = await _userManager.FindByIdAsync(item.CreatedBy);
            creatorName = item.CreatedBy == currentUser.Id ? "You" : creator?.FullName;
        }

        string? updaterName = null;
        if (item.UpdatedBy != null)
        {
            var updator = await _userManager.FindByIdAsync(item.UpdatedBy);
            updaterName = item.UpdatedBy == currentUser.Id ? "You" : updator?.FullName;
        }
        return (creatorName, updaterName);
    }
}
