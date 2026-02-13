using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockNestMVC.Data;
using StockNestMVC.DTOs.Group;
using StockNestMVC.Interfaces;
using StockNestMVC.Mappers;
using StockNestMVC.Models;
using System.Security.Claims;

namespace StockNestMVC.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public GroupRepository(ApplicationDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<GroupDto> CreateGroup(CreateGroupDto createGroupDto, AppUser user)
    {
        // check for duplicate group name
        var duplicate = await _context.UserGroup
            .Include(ug => ug.Group)
            .AnyAsync(g => g.UserId == user.Id && g.Group.Name == createGroupDto.Name);

        if (duplicate)    
        {
            throw new Exception("Group with the same name already exists");
        }

        var newGroup = new Group { Name = createGroupDto.Name };

        if (newGroup == null)
        {
            throw new Exception("There was a problem creating the group");
        }

        await _context.Groups.AddAsync(newGroup);
        await _context.SaveChangesAsync();

        if (user == null) 
        {
            throw new Exception("No user found");
        }


        // create a user group
        var userGroup = new UserGroup
        {
            UserId = user.Id,
            GroupId = newGroup.GroupId,
            Role = "Owner"
        };

        await _context.UserGroup.AddAsync(userGroup);
        await _context.SaveChangesAsync();

        return newGroup.ToGroupDto();
    }
}
