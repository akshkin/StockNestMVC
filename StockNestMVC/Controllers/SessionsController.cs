using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockNestMVC.DTOs.User;
using StockNestMVC.Interfaces;
using StockNestMVC.Services;
using Supabase.Gotrue;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StockNestMVC.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SessionsController : ControllerBase
{
    private readonly IUserSessionService _userSessionService;

    public SessionsController(IUserSessionService userSessionService)
    {
        _userSessionService = userSessionService;
    }


    // GET: api/<SessionsController>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllSessions()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var allSessions = await _userSessionService.GetAllSessions(refreshToken);

        Console.WriteLine(allSessions);

        return Ok(allSessions);
    }

    [Authorize]
    [HttpPost("revoke/{sessionId}")]
    public async Task<IActionResult> RevokeSession(int sessionId)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        bool isCurrentSession = await _userSessionService.RevokeSessionAsync(User, refreshToken, sessionId);

        Console.WriteLine($"currentSession {isCurrentSession}");


        if (isCurrentSession)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            };
            Response.Cookies.Delete("accessToken", cookieOptions);
            Response.Cookies.Delete("refreshToken", cookieOptions);
        }

        return Ok("Successfully revoked session");
    }
}
