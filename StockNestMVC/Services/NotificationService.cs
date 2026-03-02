using Microsoft.AspNetCore.Identity;
using StockNestMVC.DTOs.Notification;
using StockNestMVC.Enums;
using StockNestMVC.Interfaces;
using StockNestMVC.Mappers;
using StockNestMVC.Models;
using System.Security.Claims;

namespace StockNestMVC.Services;

public class NotificationService : INotificationService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly INotificationRepository _notificationRepo;

    public NotificationService(UserManager<AppUser> userManager, INotificationRepository notificationRepo)
    {
        _userManager = userManager;
        _notificationRepo = notificationRepo;
    }

    public async Task<IEnumerable<NotificationDto>> GetAllNotifications(ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        var notifications = await _notificationRepo.GetAllNotifications(user.Id);

        return notifications.Select(n => n.ToNotificationDto());
    }

    public async Task<IEnumerable<NotificationDto>> GetUnreadNotifications(ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        var notifications = await _notificationRepo.GetUnreadNotifications(user.Id);

        return notifications.Select(n => n.ToNotificationDto());
    }

    public async Task SetAllNotificationsAsSeen(ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        await _notificationRepo.SetAllNotificationsAsSeen(user.Id);

    }

    public async Task SetNotificationAsSeen(int notificationId, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        await _notificationRepo.SetNotificationAsSeen(notificationId, user.Id);
    }
}
