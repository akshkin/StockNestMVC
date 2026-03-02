using Microsoft.EntityFrameworkCore;
using StockNestMVC.Data;
using StockNestMVC.Enums;
using StockNestMVC.Interfaces;
using StockNestMVC.Models;

namespace StockNestMVC.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task NotifyGroupMembers(int groupId, string actingUserId, string message, NotificationType type, int? categoryId, int? itemId, string? excludedUserId)
    {
        var members = await _context.UserGroup
            .Where(ug => ug.GroupId == groupId && ug.UserId != actingUserId && ug.UserId != excludedUserId)
            .Select(ug => ug.UserId)
            .ToListAsync();

        foreach (var userId in members) 
        {
            _context.Notifications.Add(new Models.Notification
            {
                UserId = userId,
                Type = type,
                Message = message,
                GroupId = groupId,
                CategoryId = categoryId,
                ItemId = itemId,
                Seen = false,
                CreatedAt = DateTime.UtcNow,
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Notification>> GetAllNotifications(string userId)
    {
        var notifications = await _context.Notifications.Where(n => n.UserId == userId).ToListAsync();
        return notifications;
    }

    public async Task<IEnumerable<Notification>> GetUnreadNotifications(string userId)
    {
        var notifications = await _context.Notifications.Where(n => n.UserId == userId && n.Seen == false).ToListAsync();
        return notifications;
    }

    public async Task SetNotificationAsSeen(int noticationId, string userId)
    {
        var notification = await _context.Notifications
        .FirstOrDefaultAsync(n => n.Id == noticationId && n.UserId == userId);

        if (notification == null)
            throw new Exception("Notification not found");

        notification.Seen = true;

        await _context.SaveChangesAsync();
    }

    public async Task SetAllNotificationsAsSeen(string userId)
    {
        var notifications = await GetUnreadNotifications(userId);
        foreach(var notification in notifications)
        {
            notification.Seen = false;
        }
        await _context.SaveChangesAsync();
    }

    public async Task NotifyAddedRemovedMember(int groupId, string userId, string message, NotificationType type)
    {
        _context.Notifications.Add(new Models.Notification
        {
            UserId = userId,
            Type = type,
            Message = message,
            GroupId = groupId,
            Seen = false,
            CreatedAt = DateTime.UtcNow,
        });
        await _context.SaveChangesAsync();
    }
}
