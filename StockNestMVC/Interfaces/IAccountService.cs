using StockNestMVC.DTOs.User;

namespace StockNestMVC.Interfaces;

public interface IAccountService
{
    public Task<UserWithTokenDto> CreateUser(RegisterDto registerDto);

    public Task<UserWithTokenDto?> Login(LoginUserDto loginUserDto);
}
