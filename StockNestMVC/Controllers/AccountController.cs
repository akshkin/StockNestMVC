using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockNestMVC.DTOs.User;
using StockNestMVC.Interfaces;

namespace StockNestMVC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }


    //CREATE a new user
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid fields");
       
        var userWithToken = await _accountService.CreateUser(registerDto, HttpContext);            

        return Ok(userWithToken);     
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDto loginUserDto)
    { 
        var userWithToken = await _accountService.Login(loginUserDto, HttpContext);

        return Ok(userWithToken);
    }

    [HttpGet("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (refreshToken == null)
            return Unauthorized("Missing refresh token");

        await _accountService.Refresh(refreshToken, HttpContext);

        return Ok();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        await _accountService.Logout(refreshToken, HttpContext);

        return Ok("Logged out");
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await _accountService.Me(User);
        return Ok(user);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userDetails = await _accountService.GetProfile(User);
        return Ok(userDetails);
    }

    [Authorize]
    [HttpPost("update-profile")]
    public async Task<IActionResult> UpdateProfile(UpdateUserDto updateUserDto)
    {
        var userDetails = await _accountService.UpdateAccount(User, updateUserDto);

        return Ok(userDetails);
    }
}
