using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockNestMVC.DTOs.Item;
using StockNestMVC.Interfaces;
using StockNestMVC.Models;

namespace StockNestMVC.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly IItemRepository _itemRepo;
    private readonly UserManager<AppUser> _userManager;

    public ItemsController(IItemRepository itemRepo, UserManager<AppUser> userManager)
    {
        _itemRepo = itemRepo;
        _userManager = userManager;
    }

    [HttpGet("group/{groupId}/category/{categoryId}")]
    public async Task<IActionResult> GetAllItems(int groupId, int categoryId)
    {
        try
        {
            var user = await IsUserExists();

            if (user == null) return Unauthorized();

            var items = await _itemRepo.GetAll(groupId, categoryId, user);

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
            var user = await IsUserExists();

            if (user == null) return Unauthorized();

            var item = await _itemRepo.CreateItem(groupId, categoryId, user, createItemDto);

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
            var user = await IsUserExists();

            if (user == null) return Unauthorized();

            var item = await _itemRepo.GetItemById(groupId, categoryId, itemId, user);

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
            var user = await IsUserExists();

            if (user == null) return Unauthorized();

            var item = await _itemRepo.UpdateItem(groupId, categoryId, itemId, user, updateItemDto);

            if (item == null) return NotFound("Item not found");

            return Ok(item);
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }



    private async Task<AppUser?> IsUserExists()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null) return null;

        return user;
    }

    [HttpPost("group/{groupId}/category/{categoryId}/delete")]
    public async Task<IActionResult> DeleteItem(int groupId, int categoryId, List<int> itemIds)
    {
        try
        {
            var user = await IsUserExists();

            if (user == null) return Unauthorized();

            var items = await _itemRepo.DeleteItem(groupId, categoryId, itemIds, user);

            if (items == null) return NotFound("Items not found");
            return Ok("Successfully deleted items!");
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }
}
