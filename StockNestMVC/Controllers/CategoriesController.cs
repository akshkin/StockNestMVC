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

    [HttpGet("{groupId}")]
    public async Task<IActionResult> GetAllCategoriesInGroup(int groupId)
    {
        try
        {
            var user = await IsUserExists();

            if (user == null) return Unauthorized("User not found");

            var categories = await _categoryRepo.GetCategoriesInGroup(groupId, user);

            return Ok(categories);
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{groupId}/category/{categoryId}")]
    public async Task<IActionResult> GetCategoryById(int groupId, int categoryId)
    {
        try
        {
            var user = await IsUserExists();

            if (user == null) return Unauthorized();

            var category = await _categoryRepo.GetCategoryById(groupId, categoryId, user);

            if (category == null) return NotFound($"Category with id {categoryId} not found");

            return Ok(category);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("edit/{groupId}/category/{categoryId}")]
    public async Task<IActionResult> UpdateCategory(int groupId, int categoryId, CreateCategoryDto updateCategoryDto)
    {
        try
        {
            var user = await IsUserExists();

            if (user == null) return Unauthorized();

            var category = await _categoryRepo.UpdateCategory(groupId, categoryId, user, updateCategoryDto);

            if (category == null) return NotFound($"Category with id {categoryId} not found");

            return Ok(category);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("create/{groupId}")]
    public async Task<IActionResult> CreateCategory(int groupId, CreateCategoryDto createCategoryDto)
    {
        try
        {
            var user = await IsUserExists();
            if (user == null) return Unauthorized("No user found");

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
