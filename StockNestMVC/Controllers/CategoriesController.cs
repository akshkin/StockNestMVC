using Microsoft.AspNetCore.Mvc;
using StockNestMVC.DTOs.Category;

namespace StockNestMVC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{

    [HttpPost("create/{groupId}")]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto createCategoryDto)
    {
        try
        {
            return Ok();
        }
        catch (Exception ex)
        {
           
            return BadRequest(ex.Message);
            
        }
    }
}
