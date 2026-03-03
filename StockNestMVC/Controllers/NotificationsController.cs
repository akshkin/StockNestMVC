using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockNestMVC.Interfaces;
using System.Drawing;

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
        try
        {
            var notifications = await _notificationService.GetAllNotifications(User, page, size);

            return Ok(notifications);
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("unread")]
    public async Task<IActionResult> GetUnreadNotifications(int page = 1, int size = 10)
    {
        try
        {
            var unreadNotifications = await _notificationService.GetUnreadNotifications(User, page, size);
            return Ok(unreadNotifications);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{notificationId}/seen")]
    public async Task<IActionResult> SetNotificationAsSeen(int notificationId)
    {
        try
        {
            await _notificationService.SetNotificationAsSeen(notificationId, User);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("seenAll")]
    public async Task<IActionResult> SetAllNotificationsAsSeen()
    {
        try
        {
            await _notificationService.SetAllNotificationsAsSeen(User);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestNotifications()
    {
        try
        {
            var notifications = await _notificationService.GetLatestNotifications(User);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadNotificationsCount()
    {
        try
        {
            int count = await _notificationService.GetUnreadNotificationsCount(User);
            return Ok(count);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
