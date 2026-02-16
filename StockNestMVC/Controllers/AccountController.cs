using Microsoft.AspNetCore.Mvc;
using StockNestMVC.DTOs;
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

        try
        {
            var userWithToken = await _accountService.CreateUser(registerDto);
            if (userWithToken == null) return BadRequest("There was a problem creating the user");

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

        return Ok(userWithToken);
    }
}
