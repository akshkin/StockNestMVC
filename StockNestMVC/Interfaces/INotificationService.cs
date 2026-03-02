using StockNestMVC.DTOs.Notification;
using StockNestMVC.Enums;
using System.Security.Claims;

namespace StockNestMVC.Interfaces;

public interface INotificationService
{
    public Task<IEnumerable<NotificationDto>> GetAllNotifications(ClaimsPrincipal claimsPrincipal);

    public Task<IEnumerable<NotificationDto>> GetUnreadNotifications(ClaimsPrincipal claimsPrincipal);

    public Task SetNotificationAsSeen(int notificationId, ClaimsPrincipal claimsPrincipal);

    public Task SetAllNotificationsAsSeen(ClaimsPrincipal claimsPrincipal);

    public Task<IEnumerable<NotificationDto>> GetLatestNotifications(ClaimsPrincipal claimsPrincipal);

}
