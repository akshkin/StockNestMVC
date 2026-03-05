using StockNestMVC.DTOs.Category;
using System.Security.Claims;

namespace StockNestMVC.Interfaces;

public interface ICategoryService
{
    public Task<IEnumerable<CategoryDto>> GetCategoriesInGroup(int groupId, ClaimsPrincipal claimsPrincipal);

    public Task<CategoryDto?> GetCategoryById(int groupId, int categoryId, ClaimsPrincipal claimsPrincipal);

    public Task<CategoryDto> CreateCategory(int groupId, ClaimsPrincipal claimsPrincipal, CreateCategoryDto createCategoryDto);

    public Task<CategoryDto?> UpdateCategory(int groupId, int categoryId, ClaimsPrincipal claimsPrincipal, CreateCategoryDto updateCategoryDto);

    public Task<CategoryDto?> DeleteCategory(int groupId, int categoryId, ClaimsPrincipal claimsPrincipal);
}
