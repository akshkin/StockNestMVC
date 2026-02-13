using StockNestMVC.DTOs.Item;

namespace StockNestMVC.Mappers;

public static class ItemMapper
{
    public static ItemDto ToItemDto(this ItemDto itemDto)
    {
        return new ItemDto
        {
            ItemId = itemDto.ItemId,
            Name = itemDto.Name,
            Quantity = itemDto.Quantity,
        };
    }
}
