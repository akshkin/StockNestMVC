using StockNestMVC.Enums;

namespace StockNestMVC.DTOs.Notification;

public class NotificationDto
{
    public int Id { get; set; }
    public string Message { get; set; }

    public bool Seen { get; set; }

    public DateTime CreatedAt { get; set; }

    public NotificationType Type { get; set; }

    public int? GroupId { get; set; }
    public int? CategoryId { get; set; }
    public int? ItemId { get; set; }
}
