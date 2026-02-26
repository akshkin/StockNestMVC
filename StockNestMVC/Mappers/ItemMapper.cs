using StockNestMVC.DTOs.Item;
using StockNestMVC.Models;

namespace StockNestMVC.Mappers;

public static class ItemMapper
{
    public static ItemDto ToItemDto(this Item item, string creator, string updator)
    {
        return new ItemDto
        {
            ItemId = item.ItemId,
            Name = item.Name,
            Quantity = item.Quantity,
            CreatedAt = item.CreatedAt,
            CreatedBy = creator,
            UpdatedAt = item.UpdatedAt,
            UpdatedBy = updator,
        };
    }
}
