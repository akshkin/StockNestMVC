using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockNestMVC.Models;

namespace StockNestMVC.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StatsController : ControllerBase
{

    private readonly UserManager<AppUser> _userManager;

    public StatsController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null) return null;
        return Ok();
    }
}
