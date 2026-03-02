using StockNestMVC.Enums;
using StockNestMVC.Models;

namespace StockNestMVC.Interfaces;

public interface INotificationRepository
{
    public Task NotifyGroupMembers(int groupId, string actingUserId, string message, NotificationType type, int? categoryId = null, int? itemId = null);

    public Task<IEnumerable<Notification>> GetAllNotifications(string userId);

    public Task<IEnumerable<Notification>> GetUnreadNotifications(string userId);

    public Task SetNotificationAsSeen(int noticationId, string userId);

    public Task SetAllNotificationsAsSeen(string userId);
}
