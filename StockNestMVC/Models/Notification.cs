using StockNestMVC.Enums;

namespace StockNestMVC.Models;

public class Notification
{   
    public int Id { get; set; }

    public string UserId { get; set; }
    public AppUser User { get; set; }

    public int? GroupId { get; set; }
    public Group Group { get; set; }

    public int? CategoryId { get; set; }
    public Category Category { get; set; }

    public int? ItemId { get; set; }
    public Item Item { get; set; }

    public NotificationType Type { get; set; }
    public string Message { get; set; }

    public bool Seen { get; set; }
    public DateTime CreatedAt { get; set; }

}
