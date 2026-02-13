using StockNestMVC.DTOs.Category;

namespace StockNestMVC.Mappers;

public static class CategoryMapper
{
    public static CategoryDto ToCategoryDto(this CategoryDto categoryDto)
    {
        return new CategoryDto
        {
            CategoryId = categoryDto.CategoryId,
            Name = categoryDto.Name,
        };
    }
}
