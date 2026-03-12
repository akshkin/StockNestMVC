using StockNestMVC.DTOs;
using StockNestMVC.DTOs.User;
using StockNestMVC.Models;
using System.Security.Claims;

namespace StockNestMVC.Interfaces;

public interface IAccountService
{
    public Task<UserWithTokenDto> CreateUser(RegisterDto registerDto, HttpContext http);

    public Task<UserWithTokenDto?> Login(LoginUserDto loginUserDto, HttpContext http);

    public Task Refresh(string refreshToken, HttpContext http);

    public Task Logout(string refreshToken, HttpContext http);

    public Task<CurrentUserDto> Me(ClaimsPrincipal claimsPrincipal);

    public Task<UserDto> GetProfile(ClaimsPrincipal claimsPrincipal);
    public Task<UserDto> UpdateAccount(ClaimsPrincipal claimsPrincipal, UpdateUserDto updateUserDto);

    public Task<AuthResponseDto> GenRefreshToken(AppUser user);

    public Task RemoveRefreshToken(AppUser user);
}
