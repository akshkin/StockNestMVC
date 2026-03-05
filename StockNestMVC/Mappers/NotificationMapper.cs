using StockNestMVC.DTOs.Notification;
using StockNestMVC.Models;

namespace StockNestMVC.Mappers;

public static class NotificationMapper
{
    public static NotificationDto ToNotificationDto(this Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            GroupId = notification.GroupId,
            Seen =  notification.Seen,
            CategoryId = notification.CategoryId,
            ItemId = notification.ItemId,
            Message = notification.Message,
            CreatedAt = notification.CreatedAt,
            Type = notification.Type,
        };
    }
}
