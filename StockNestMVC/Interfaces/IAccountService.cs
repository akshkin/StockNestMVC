using StockNestMVC.DTOs;
using StockNestMVC.DTOs.User;
using StockNestMVC.Models;

namespace StockNestMVC.Interfaces;

public interface IAccountService
{
    public Task<UserWithTokenDto> CreateUser(RegisterDto registerDto);

    public Task<UserWithTokenDto?> Login(LoginUserDto loginUserDto);

    public Task<AuthResponseDto> GenRefreshToken(AppUser user);

    public Task RemoveRefreshToken(AppUser user);
}
