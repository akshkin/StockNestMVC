using StockNestMVC.DTOs;
using StockNestMVC.Models;

namespace StockNestMVC.Mappers;

public static class UserMapper
{
    public static UserDto ToUserDto(this AppUser user) 
    {
        return new UserDto
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.FullName,
            Email = user.Email
        };
    }

    public static AppUser ToAppUserFromRegisterDto(this RegisterDto registerDto)
    {
        return new AppUser
        {
            UserName = registerDto.FirstName + registerDto.LastName,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.EmailAddress
        };
    }


}
