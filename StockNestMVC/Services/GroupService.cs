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
        var duplicate = await _groupRepo.CheckDuplicateGroup(user, createGroupDto.Name, null);

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
            bool isUserCreator = group.Group.CreatedBy == user.Id;
            if (group.Group.CreatedBy != null)
            {
                var creator = await _userManager.FindByIdAsync(group.Group.CreatedBy);
                creatorName = isUserCreator? "You" : creator?.FullName;
            }

            string updatorName = null;
            bool isUserUpdator = group.Group.UpdatedBy == user.Id;
            if (group.Group.UpdatedBy != null) 
            {
                var updator = await _userManager.FindByIdAsync(group.Group.UpdatedBy);
                updatorName= isUserUpdator? "You":updator?.FullName;
            }
            groupDtos.Add(group.Group.ToGroupDto(group.Role, creatorName, updatorName));       

        }
        
        return groupDtos;
    }

    public async Task<GroupDto?> UpdateGroup(int groupId, CreateGroupDto updateGroupDto, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        var existingGroup = await _groupRepo.GetGroupById(groupId, user);

        if (existingGroup == null)
        {
            throw new Exception($"Group with id {groupId} not found");
        }

        var roleInGroup = await _groupRepo.GetRoleInGroup(groupId, user);
    
        var duplicate = await _groupRepo.CheckDuplicateGroup(user, existingGroup.Name, groupId);

        if (duplicate) throw new Exception("Group with this name already exists");

        existingGroup.Name = updateGroupDto.Name;
        existingGroup.UpdatedBy = user.Id;
        existingGroup.UpdatedAt = DateTime.UtcNow;

        await _groupRepo.UpdateGroup(existingGroup);

        return existingGroup.ToGroupDto(roleInGroup, existingGroup.CreatedBy, existingGroup.UpdatedBy);
    }

    public async Task<GroupDto?> GetGroupById(int groupId, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        Group group = await _groupRepo.GetGroupById(groupId, user);

        if (group == null) throw new Exception("Group not found");

        string creatorName = null;
        bool isUserCreator = group.CreatedBy == user.Id;
        if (group.CreatedBy != null)
        {
            var creator = await _userManager.FindByIdAsync(group.CreatedBy);
            creatorName = isUserCreator ? "You" : creator?.FullName;
        }

        string updatorName = null;
        bool isUserUpdator = group.UpdatedBy == user.Id;
        if (group.UpdatedBy != null)
        {
            var updator = await _userManager.FindByIdAsync(group.UpdatedBy);
            updatorName = isUserUpdator ? "You" : updator?.FullName;
        }

        var roleInGroup = await _groupRepo.GetRoleInGroup(groupId, user);

        return group.ToGroupDto(roleInGroup, creatorName, updatorName);
    }

    public async Task<GroupDto?> DeleteGroup(int groupId, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        var existingGroup = await _groupRepo.GetGroupById(groupId, user);

        if (existingGroup == null) return null;

        // check if user is the owner
        var role = await _groupRepo.GetRoleInGroup(groupId, user);

        if (role == "Owner" || role == "Co-Owner")
        {
            await _groupRepo.DeleteGroup(existingGroup);
        }
        else
        {
            throw new Exception("Only the group owner can delete the group");
        }
        
        return existingGroup.ToGroupDto(role, null, null);
    }
}
