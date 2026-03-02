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
    public async Task<IActionResult> GetAllNotifications()
    {
        try
        {
            var notifications = await _notificationService.GetAllNotifications(User);

            return Ok(notifications);
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("unread")]
    public async Task<IActionResult> GetUnreadNotifications()
    {
        try
        {
            var unreadNotifications = await _notificationService.GetUnreadNotifications(User);
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
}
