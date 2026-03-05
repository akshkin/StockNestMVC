using System.ComponentModel.DataAnnotations;

namespace StockNestMVC.DTOs.Item;

public class ItemDto
{
    public int ItemId { get; set; }

    public string Name { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
}
