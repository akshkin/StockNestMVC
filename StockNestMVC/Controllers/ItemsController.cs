using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

    public IActionResult Index()
    {
        return Ok();
    }
}
