using StockNestMVC.DTOs.Item;
using StockNestMVC.Models;

namespace StockNestMVC.Mappers;

public static class ItemMapper
{
    public static ItemDto ToItemDto(this Item item)
    {
        return new ItemDto
        {
            ItemId = item.ItemId,
            Name = item.Name,
            Quantity = item.Quantity,
        };
    }
}
