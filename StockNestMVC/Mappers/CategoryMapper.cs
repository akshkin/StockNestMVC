using StockNestMVC.DTOs.Category;
using StockNestMVC.Models;

namespace StockNestMVC.Mappers;

public static class CategoryMapper
{
    public static CategoryDto ToCategoryDto(this Category category)
    {
        return new CategoryDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
        };
    }
}
