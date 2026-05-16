using Microsoft.AspNetCore.Identity;
using StockNestMVC.DTOs;
using StockNestMVC.DTOs.User;
using StockNestMVC.Exceptions;
using StockNestMVC.Interfaces;
using StockNestMVC.Mappers;
using StockNestMVC.Models;
using System.Security.Claims;

namespace StockNestMVC.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IUserRepository _userRepo;
    private readonly IUserSessionService _userSessionService;

    public AccountService(
        UserManager<AppUser> userManager, 
        ITokenService tokenService, 
        SignInManager<AppUser> signInManager, 
        IUserRepository userRepo,
        IUserSessionService userSessionService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _signInManager = signInManager;
        _userRepo = userRepo;
        _userSessionService = userSessionService;
    }

    public async Task<UserWithTokenDto> CreateUser(RegisterDto registerDto, HttpContext http, string deviceName)
    {
        try
        {
            var newUser = registerDto.ToAppUserFromRegisterDto();

            var createdUser = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (createdUser.Succeeded)
            {
                var userDto = newUser.ToUserDto();
                var userWithToken = new UserWithTokenDto
                {
                    User = userDto,
                    Token = await _tokenService.CreateToken(newUser)
                };
               
                var authResponse = await GenRefreshToken(newUser);
                // User is valid, now create session
                var session = await _userSessionService.CreateSessionAsync(
                    newUser.Id,
                    deviceName,
                    http.Connection.RemoteIpAddress?.ToString(),
                    authResponse.RefreshToken
                );

                await GenerateTokens(newUser, http, authResponse);
                return userWithToken;
            }
            else
            {
                var errors = createdUser.Errors.Select(e => e.Description);
                throw new BadRequestException(string.Join(", ", errors));
            }
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task<UserWithTokenDto?> Login(LoginUserDto loginUserDto, HttpContext http, string deviceName)
    {
        var existingUser = await _userManager.FindByEmailAsync(loginUserDto.Email);

        if (existingUser == null) 
            throw new UnauthorizedException("Invalid email or password");

        var passwordConfirmed = await _signInManager.CheckPasswordSignInAsync(existingUser, loginUserDto.Password, false);

        if (!passwordConfirmed.Succeeded)
            throw new UnauthorizedException("Invalid email or password");

        var authResponse = await GenRefreshToken(existingUser);

        // User is valid, now create session
        var session = await _userSessionService.CreateSessionAsync(
            existingUser.Id,
            deviceName,
            http.Connection.RemoteIpAddress?.ToString(),
            authResponse.RefreshToken
        );

        await GenerateTokens(existingUser, http, authResponse);

        return new UserWithTokenDto
        {
            User = existingUser.ToUserDto(),
            Token = await _tokenService.CreateToken(existingUser)
        };
    }

    public async Task Refresh(string refreshToken, HttpContext http)
    {
        var session = await _userSessionService.GetSessionByRefreshTokenAsync(refreshToken);
        if (session == null)
            throw new UnauthorizedException("Invalid or expired session");

        var user = session.AppUser;

        if (user == null)
        {
            throw new UnauthorizedException("User not found");
        }
        else
        {
            var authResponse = await GenRefreshToken(user);
            await GenerateTokens(user, http, authResponse);
            // Update last activity
            await _userSessionService.UpdateLastActivityAsync(session, authResponse.RefreshToken);
        }
    }

    public async Task Logout(string refreshToken, HttpContext http)
    {
        if (refreshToken == null) throw new UnauthorizedException("Missing refresh token");

        var session = await _userSessionService.GetSessionByRefreshTokenAsync(refreshToken);
        if (session == null)
            throw new UnauthorizedException("Invalid or expired session");

        var user = session.AppUser;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        };

        await _userSessionService.RevokeSessionById(session.UserSessionId, user.Id);
        http.Response.Cookies.Delete("accessToken", cookieOptions);
        http.Response.Cookies.Delete("refreshToken", cookieOptions);

    }

    public async Task<CurrentUserDto> Me(ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user == null) throw new  UnauthorizedException("No user found");

        return new CurrentUserDto
        { 
            User = claimsPrincipal.Identity.IsAuthenticated, 
            Name = user.FirstName 
        };
    }

    public async Task<UserDto> GetProfile(ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) 
            throw new UnauthorizedException("No user found");

        return user.ToUserDto();
    }

    public async Task<UserDto> UpdateAccount(ClaimsPrincipal claimsPrincipal, UpdateUserDto updateUserDto)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) 
            throw new UnauthorizedException("No user found");

        user.FirstName = updateUserDto.FirstName;
        user.LastName = updateUserDto.LastName;
        user.ProfileImageUrl = updateUserDto.ProfileImageUrl;

        await _userManager.UpdateAsync(user);

        return user.ToUserDto();
    }

    public async Task<AuthResponseDto> GenRefreshToken(AppUser user)
    {
        var newAccessToken = await _tokenService.CreateToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        var authResponse = new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };

        return authResponse;
    }

    public async Task RemoveRefreshToken(AppUser user)
    {
        await _userRepo.Logout(user);
    }

    private async Task GenerateTokens(AppUser user, HttpContext http, AuthResponseDto authResponse, int? sessionId = null, bool isRefresh = false)
    {
        var newAccessToken = authResponse.AccessToken;
        var newRefreshToken = authResponse.RefreshToken;

        http.Response.Cookies.Append("accessToken", newAccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(15)
        });

        http.Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(1) // change later to 2 days?           
        });
    }
}
