using StockNestMVC.DTOs.Category;
using StockNestMVC.Interfaces;

namespace StockNestMVC.Repositories;

public class CategoryRepository : ICategoryRepository
{
    public async Task<CategoryDto> CreateCategory(CreateCategoryDto createCategoryDto)
    {
        throw new NotImplementedException();
    }
}
