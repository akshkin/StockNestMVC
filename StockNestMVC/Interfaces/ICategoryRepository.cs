using StockNestMVC.DTOs.Category;
using StockNestMVC.Models;

namespace StockNestMVC.Interfaces;

public interface ICategoryRepository
{
    public Task<IEnumerable<CategoryDto>> GetCategoriesInGroup(int groupId, AppUser user);

    public Task<CategoryDto?> GetCategoryById(int groupId, int categoryId, AppUser user);

    public Task<CategoryDto> CreateCategory(int groupId, AppUser user, CreateCategoryDto createCategoryDto);
}
