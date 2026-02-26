using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockNestMVC.DTOs.Category;
using StockNestMVC.Interfaces;
using StockNestMVC.Models;

namespace StockNestMVC.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("group/{groupId}")]
    public async Task<IActionResult> GetAllCategoriesInGroup(int groupId)
    {
        try
        {
            var categories = await _categoryService.GetCategoriesInGroup(groupId, User);

            return Ok(categories);
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("group/{groupId}/category/{categoryId}")]
    public async Task<IActionResult> GetCategoryById(int groupId, int categoryId)
    {
        try
        {
            var category = await _categoryService.GetCategoryById(groupId, categoryId, User);

            if (category == null) return NotFound($"Category with id {categoryId} not found");

            return Ok(category);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("group/{groupId}/category/{categoryId}/edit")]
    public async Task<IActionResult> UpdateCategory(int groupId, int categoryId, CreateCategoryDto updateCategoryDto)
    {
        try
        {
            var category = await _categoryService.UpdateCategory(groupId, categoryId, User, updateCategoryDto);

            if (category == null) return NotFound($"Category with id {categoryId} not found");

            return Ok(category);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("group/{groupId}/create")]
    public async Task<IActionResult> CreateCategory(int groupId, CreateCategoryDto createCategoryDto)
    {
        try
        {
            var category = await _categoryService.CreateCategory(groupId, User, createCategoryDto);
            return Ok(category);
        }
        catch (Exception ex)
        {           
            return BadRequest(ex.Message);            
        }
    }

    [HttpPost("group/{groupId}/category/{categoryId}/delete")]
    public async Task<IActionResult> DeleteCategory(int groupId, int categoryId)
    {
        try
        {
            var category = await _categoryService.DeleteCategory(groupId, categoryId, User);
            if (category == null) return NotFound($"Category with id {categoryId} not found");

            return Ok("Successfully deleted category");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
