using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockNestMVC.DTOs.Item;
using StockNestMVC.Interfaces;

namespace StockNestMVC.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemsController(IItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpGet("group/{groupId}/category/{categoryId}")]
    public async Task<IActionResult> GetAllItems(int groupId, int categoryId, int page = 1, int size = 10)
    {        
        var items = await _itemService.GetAll(groupId, categoryId, User, page, size);

        return Ok(items);
    }

    [HttpPost("group/{groupId}/category/{categoryId}/create")]
    public async Task<IActionResult> CreateItem(int groupId, int categoryId, CreateItemDto createItemDto )
    {
        var item = await _itemService.CreateItem(groupId, categoryId, User, createItemDto);

        return Ok(item);
    }

    [HttpGet("group/{groupId}/category/{categoryId}/item/{itemId}")]
    public async Task<IActionResult> GetItemById(int groupId, int categoryId, int itemId)
    {
        var item = await _itemService.GetItemById(groupId, categoryId, itemId, User);

        if (item == null) return NotFound("Item not found");

        return Ok(item);
    }

    [HttpPost("group/{groupId}/category/{categoryId}/item/{itemId}/edit")]
    public async Task<IActionResult> UpdateItem(int groupId, int categoryId, int itemId, CreateItemDto updateItemDto)
    {
        var item = await _itemService.UpdateItem(groupId, categoryId, itemId, User, updateItemDto);

        if (item == null) return NotFound("Item not found");

        return Ok(item);
       
    }


    [HttpPost("group/{groupId}/category/{categoryId}/delete")]
    public async Task<IActionResult> DeleteItem(int groupId, int categoryId, List<int> itemIds)
    {
        var items = await _itemService.DeleteItem(groupId, categoryId, itemIds, User);

        if (items == null) return NotFound("Items not found");
        return Ok("Successfully deleted items!");
    }

    [HttpGet("group/{groupId}/category/{categoryId}/item/{itemId}/page-index")]
    public async Task<IActionResult> GetItemPageIndex(int groupId, int categoryId, int itemId)
    {
        var pageIndex = await _itemService.GetPageIndex(User, groupId, categoryId, itemId);
        return Ok(pageIndex);       
    }
}
