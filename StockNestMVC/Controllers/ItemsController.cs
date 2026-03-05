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
    public async Task<IActionResult> GetAllItems(int groupId, int categoryId)
    {
        try
        {
            var items = await _itemService.GetAll(groupId, categoryId, User);

            return Ok(items);
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("group/{groupId}/category/{categoryId}/create")]
    public async Task<IActionResult> CreateItem(int groupId, int categoryId, CreateItemDto createItemDto )
    {
        try
        {
            var item = await _itemService.CreateItem(groupId, categoryId, User, createItemDto);

            return Ok(item);
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("group/{groupId}/category/{categoryId}/item/{itemId}")]
    public async Task<IActionResult> GetItemById(int groupId, int categoryId, int itemId)
    {
        try
        {
            var item = await _itemService.GetItemById(groupId, categoryId, itemId, User);

            if (item == null) return NotFound("Item not found");

            return Ok(item);
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("group/{groupId}/category/{categoryId}/item/{itemId}/edit")]
    public async Task<IActionResult> UpdateItem(int groupId, int categoryId, int itemId, CreateItemDto updateItemDto)
    {
        try
        {
            var item = await _itemService.UpdateItem(groupId, categoryId, itemId, User, updateItemDto);

            if (item == null) return NotFound("Item not found");

            return Ok(item);
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPost("group/{groupId}/category/{categoryId}/delete")]
    public async Task<IActionResult> DeleteItem(int groupId, int categoryId, List<int> itemIds)
    {
        try
        {
            var items = await _itemService.DeleteItem(groupId, categoryId, itemIds, User);

            if (items == null) return NotFound("Items not found");
            return Ok("Successfully deleted items!");
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }
}
