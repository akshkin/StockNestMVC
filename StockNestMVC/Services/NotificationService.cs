using Microsoft.AspNetCore.Identity;
using StockNestMVC.DTOs;
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

    public async Task<PaginatedResultDto<NotificationDto>> GetAllNotifications(ClaimsPrincipal claimsPrincipal, int page, int size)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        var (notifications, total) = await _notificationRepo.GetAllNotifications(user.Id, page, size);

        //return notifications.Select(n => n.ToNotificationDto());
        bool HasNextPage = (page * size) < total;

        return new PaginatedResultDto<NotificationDto>
        {
            Items = notifications.Select(n => n.ToNotificationDto()),
            TotalCount = total,
            PageNumber = page,
            PageSize = size,
            HasNextPage = HasNextPage
        };
    }

    public async Task<PaginatedResultDto<NotificationDto>> GetUnreadNotifications(ClaimsPrincipal claimsPrincipal, int page, int size)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        var (notifications, total) = await _notificationRepo.GetUnreadNotifications(user.Id, page, size);

        bool HasNextPage = (page * size) < total;

        return new PaginatedResultDto<NotificationDto>
        {
            Items = notifications.Select(n => n.ToNotificationDto()),
            TotalCount = total,
            PageNumber = page,
            PageSize = size,
            HasNextPage = HasNextPage
        };

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

    public async Task<IEnumerable<NotificationDto>> GetLatestNotifications(ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        var notifications = await _notificationRepo.GetLatestNotifications(7, user.Id);

        return notifications.Select(n => n.ToNotificationDto());
    }
}
