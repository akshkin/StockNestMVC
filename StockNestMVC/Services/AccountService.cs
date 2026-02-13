using Microsoft.AspNetCore.Identity;
using StockNestMVC.DTOs;
using StockNestMVC.Interfaces;
using StockNestMVC.Mappers;
using StockNestMVC.Models;

namespace StockNestMVC.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    public AccountService(UserManager<AppUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<UserWithTokenDto> CreateUser(RegisterDto registerDto)
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
                return userWithToken;
            }
            else
            {
                var errors = createdUser.Errors.Select(e => e.Description);
                throw new Exception(string.Join(", ", errors));
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
