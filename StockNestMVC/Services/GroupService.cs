using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockNestMVC.DTOs.Group;
using StockNestMVC.Interfaces;
using StockNestMVC.Mappers;
using StockNestMVC.Models;
using System.Security.Claims;

namespace StockNestMVC.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepo;
    private readonly UserManager<AppUser> _userManager;

    public GroupService(IGroupRepository groupRepo, UserManager<AppUser> userManager)
    {
        _groupRepo = groupRepo;
        _userManager = userManager;
    }

    public async Task<GroupDto> CreateGroup(CreateGroupDto createGroupDto, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        // Check duplicate       
        var duplicate = await _groupRepo.CheckDuplicateGroup(user, createGroupDto.Name);

        if (duplicate)
            throw new Exception("Group with the same name already exists");

        var group = new Group
        {
            Name = createGroupDto.Name,
            CreatedBy = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _groupRepo.CreateGroup(group);

        // Add membership
        var membership = new UserGroup
        {
            UserId = user.Id,
            GroupId = group.GroupId,
            Role = "Owner"
        };

        await _groupRepo.CreateUserGroup(membership);

        return group.ToGroupDto("Owner", user.FullName, null);
    }

    public async Task<IEnumerable<GroupDto>> GetGroups(ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        var groups = await _groupRepo.GetAllUserGroups(user);

        var groupDtos = new List<GroupDto>();

        foreach(var group in groups)
        {
            string creatorName = null;
            bool isUserCreator = group.UserId == user.Id;
            if (group.Group.CreatedBy != null)
            {
                var creator = await _userManager.FindByIdAsync(group.Group.CreatedBy);
                creatorName = isUserCreator? "You" : creator?.FullName;
            }

            string updatorName = null;
            bool isUserUpdator = group.UserId == user.Id;
            if (group.Group.UpdatedBy != null) 
            {
                var updator = await _userManager.FindByIdAsync(group.Group.UpdatedBy);
                updatorName= isUserUpdator? "You":updator?.FullName;
            }
            groupDtos.Add(group.Group.ToGroupDto(group.Role, creatorName, updatorName));       

        }
        
        return groupDtos;
    }
}
