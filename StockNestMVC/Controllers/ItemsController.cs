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

    private async Task<AppUser?> IsUserExists()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null) return null;

        return user;
    }
}
