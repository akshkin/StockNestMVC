using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockNestMVC.DTOs.Category;
using StockNestMVC.Interfaces;
using StockNestMVC.Models;

namespace StockNestMVC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepo;
    private readonly UserManager<AppUser> _userManager;
public CategoriesController(ICategoryRepository categoryRepo, UserManager<AppUser> userManager)
    {
        _categoryRepo = categoryRepo;
        _userManager = userManager;
    }

    [HttpPost("create/{groupId}")]
    public async Task<IActionResult> CreateCategory(int groupId, CreateCategoryDto createCategoryDto)
    {
        try
        {
            var user = await IsUserExists();
            if (user == null) return BadRequest("No user found");

            var category = await _categoryRepo.CreateCategory(groupId, user, createCategoryDto);
            return Ok(category);
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
