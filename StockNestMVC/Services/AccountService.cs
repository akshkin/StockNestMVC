using Microsoft.AspNetCore.Identity;
using StockNestMVC.DTOs.User;
using StockNestMVC.Interfaces;
using StockNestMVC.Mappers;
using StockNestMVC.Models;

namespace StockNestMVC.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountService(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _signInManager = signInManager;
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

    public async Task<UserWithTokenDto?> Login(LoginUserDto loginUserDto)
    {
        var existingUser = await _userManager.FindByEmailAsync(loginUserDto.Email);

        if (existingUser == null) return null;

        var passwordConfirmed = await _signInManager.CheckPasswordSignInAsync(existingUser, loginUserDto.Password, false);

        if (!passwordConfirmed.Succeeded) return null;

        return new UserWithTokenDto
        {
            User = existingUser.ToUserDto(),
            Token = await _tokenService.CreateToken(existingUser)
        };
    }
}
