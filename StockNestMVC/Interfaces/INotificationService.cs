using StockNestMVC.DTOs;
using StockNestMVC.DTOs.Notification;
using StockNestMVC.Enums;
using System.Security.Claims;

namespace StockNestMVC.Interfaces;

public interface INotificationService
{
    public Task<PaginatedResultDto<NotificationDto>> GetAllNotifications(ClaimsPrincipal claimsPrincipal, int page, int size);

    public Task<PaginatedResultDto<NotificationDto>> GetUnreadNotifications(ClaimsPrincipal claimsPrincipal, int page, int size);

    public Task SetNotificationAsSeen(int notificationId, ClaimsPrincipal claimsPrincipal);

    public Task SetAllNotificationsAsSeen(ClaimsPrincipal claimsPrincipal);

    public Task<IEnumerable<NotificationDto>> GetLatestNotifications(ClaimsPrincipal claimsPrincipal);

}
