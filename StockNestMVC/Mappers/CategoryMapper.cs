using StockNestMVC.DTOs.Category;
using StockNestMVC.Models;

namespace StockNestMVC.Mappers;

public static class CategoryMapper
{
    public static CategoryDto ToCategoryDto(this Category category, string creator, string updator)
    {
        return new CategoryDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            CreatedBy = creator,
            CreatedAt = category.CreatedAt,
            UpdatedBy = updator,
            UpdatedAt = category.UpdatedAt,
        };
    }
}
