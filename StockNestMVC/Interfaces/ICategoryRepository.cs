using StockNestMVC.DTOs.Category;
using StockNestMVC.Models;

namespace StockNestMVC.Interfaces;

public interface ICategoryRepository
{
    public Task<CategoryDto> CreateCategory(int groupId, AppUser user, CreateCategoryDto createCategoryDto);
}
