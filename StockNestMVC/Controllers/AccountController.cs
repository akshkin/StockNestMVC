using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockNestMVC.DTOs;
using StockNestMVC.DTOs.User;
using StockNestMVC.Interfaces;
using StockNestMVC.Models;

namespace StockNestMVC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly UserManager<AppUser> _userManager;

    public AccountController(IAccountService accountService, UserManager<AppUser> userManager)
    {
        _accountService = accountService;
        _userManager = userManager;
    }


    //CREATE a new user
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid fields");

        try
        {
            var userWithToken = await _accountService.CreateUser(registerDto);

            if (userWithToken == null) return BadRequest("There was a problem creating the user");

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userWithToken.User.UserId);

            if (user == null) return Unauthorized("No user found");

            await GenerateTokens(user);


            return Ok(userWithToken);
        } 
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDto loginUserDto)
    { 
        var userWithToken = await _accountService.Login(loginUserDto);

        if (userWithToken == null) return BadRequest("Invalid email or password");

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userWithToken.User.UserId);

        if (user == null) return Unauthorized("No user found");

        await GenerateTokens(user);


        return Ok(userWithToken);
    }

    [HttpGet("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (refreshToken == null)
            return Unauthorized("Missing refresh token");

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            return Unauthorized("Invalid or expired refresh token");

        } else
        {
            await GenerateTokens(user);

            return Ok();
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (refreshToken != null)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user != null) 
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = DateTime.MinValue;
                await _accountService.RemoveRefreshToken(user);
                await _userManager.UpdateAsync(user);
            }
        }

        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");

        return Ok("Logged out");
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new { user = User.Identity.IsAuthenticated });
    }

    private async Task GenerateTokens(AppUser user)
    {
        var authResponse = await _accountService.GenRefreshToken(user);
        var newAccessToken = authResponse.AccessToken;
        var newRefreshToken = authResponse.RefreshToken;

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

        await _userManager.UpdateAsync(user);

        Response.Cookies.Append("accessToken", newAccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(15) 
        });

        Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(1) // change later to 2 days?           
        });
    }
}
