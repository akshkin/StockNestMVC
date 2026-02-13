using StockNestMVC.DTOs.Category;

namespace StockNestMVC.Interfaces;

public interface ICategoryRepository
{
    public Task<CategoryDto> CreateCategory(CreateCategoryDto createCategoryDto);
}
