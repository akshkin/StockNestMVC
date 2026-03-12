using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockNestMVC.Interfaces;

namespace StockNestMVC.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllNotifications(int page = 1, int size = 10)
    {        
        var notifications = await _notificationService.GetAllNotifications(User, page, size);

        return Ok(notifications);       
    }

    [HttpGet("unread")]
    public async Task<IActionResult> GetUnreadNotifications(int page = 1, int size = 10)
    {       
        var unreadNotifications = await _notificationService.GetUnreadNotifications(User, page, size);
        return Ok(unreadNotifications);
    }

    [HttpPost("{notificationId}/seen")]
    public async Task<IActionResult> SetNotificationAsSeen(int notificationId)
    {
        await _notificationService.SetNotificationAsSeen(notificationId, User);

        return Ok();
    }

    [HttpPost("seenAll")]
    public async Task<IActionResult> SetAllNotificationsAsSeen()
    {
        await _notificationService.SetAllNotificationsAsSeen(User);

        return Ok();
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestNotifications()
    {
        var notifications = await _notificationService.GetLatestNotifications(User);
        return Ok(notifications);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadNotificationsCount()
    {
        int count = await _notificationService.GetUnreadNotificationsCount(User);
        return Ok(count);
    }
}
