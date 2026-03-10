using StockNestMVC.DTOs;
using StockNestMVC.DTOs.User;
using StockNestMVC.Models;
using System.Security.Claims;

namespace StockNestMVC.Interfaces;

public interface IAccountService
{
    public Task<UserWithTokenDto> CreateUser(RegisterDto registerDto);

    public Task<UserWithTokenDto?> Login(LoginUserDto loginUserDto);

    public Task<UserDto> GetProfile(ClaimsPrincipal claimsPrincipal);
    public Task<UserDto> UpdateAccount(ClaimsPrincipal claimsPrincipal, UpdateUserDto updateUserDto);

    public Task<AuthResponseDto> GenRefreshToken(AppUser user);

    public Task RemoveRefreshToken(AppUser user);
}
