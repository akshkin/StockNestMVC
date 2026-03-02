using Microsoft.AspNetCore.Identity;
using StockNestMVC.DTOs.Group;
using StockNestMVC.Interfaces;
using StockNestMVC.Mappers;
using StockNestMVC.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace StockNestMVC.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepo;
    private readonly UserManager<AppUser> _userManager;
    private readonly INotificationRepository _notificationRepo;

    public GroupService(IGroupRepository groupRepo, UserManager<AppUser> userManager, INotificationRepository notificationRepo)
    {
        _groupRepo = groupRepo;
        _userManager = userManager;
        _notificationRepo = notificationRepo;
    }

    public async Task<GroupDto> CreateGroup(CreateGroupDto createGroupDto, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        // Check duplicate       
        var duplicate = await _groupRepo.CheckDuplicateGroup(user, createGroupDto.Name, null);

        if (duplicate)
            throw new Exception("Group with the same name already exists");

        var group = new Models.Group
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

        string message = $"{user.FullName} updated group name {existingGroup.Name} to {updateGroupDto.Name}";

        await _groupRepo.UpdateGroup(existingGroup);

        await _notificationRepo.NotifyGroupMembers(groupId, user.Id, message, Enums.NotificationType.GroupUpdated, groupId);

        return existingGroup.ToGroupDto(roleInGroup, existingGroup.CreatedBy, existingGroup.UpdatedBy);
    }

    public async Task<GroupDto?> GetGroupById(int groupId, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        var group = await _groupRepo.GetGroupById(groupId, user);

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

        string message = $"{user.FullName} deleted group {existingGroup.Name}";

        await _notificationRepo.NotifyGroupMembers(groupId, user.Id, message, Enums.NotificationType.GroupDeleted, groupId);

        return existingGroup.ToGroupDto(role, null, null);
    }

    public async Task InviteGroupMember(int groupId, ClaimsPrincipal claimsPrincipal, string email, string role)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        var group = await _groupRepo.GetGroupById(groupId, user);

        if (group == null) throw new Exception("Group not found");

        var invitedUser = await _userManager.FindByEmailAsync(email);

        if (invitedUser == null) throw new Exception("User with entered email address does not exist in our database");

        var isAlreadyMemeber = await _groupRepo.CheckIfMemberInGroup(groupId, invitedUser);

        if(isAlreadyMemeber)
        {
            throw new Exception("User is already  amember of this group");
        }

        var roleInGroup = await _groupRepo.GetRoleInGroup(groupId, user);

       
        if (roleInGroup == "Owner" || roleInGroup == "Co-Owner")
        {
            // Add to group
            var newMembership = new UserGroup
            {
                UserId = invitedUser.Id,
                GroupId = groupId,
                Role = role
            };
            await _groupRepo.InviteUser(newMembership);

            // notify added member
            string addedUserMessage = $"{user.FullName} added you to group {group.Name}";
            await _notificationRepo.NotifyAddedRemovedMember(groupId, invitedUser.Id, addedUserMessage, Enums.NotificationType.UserRemovedFromGroup);

            // notify other members
            string message = $"{user.FullName} added {invitedUser.FullName} to group {group.Name}";           
            await _notificationRepo.NotifyGroupMembers(groupId, user.Id, message, Enums.NotificationType.UserJoinedGroup, null, null, invitedUser.Id);
        }
        else
            throw new Exception("Only the group owner can invite users");
    }

    public async Task<GroupMemberResponseDto> GetGroupMembers(int groupId, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        var userGroup = await _groupRepo.GetUserGroup(groupId, user);

        if (userGroup == null) throw new Exception("Group not found");

        var members = await _groupRepo.GetGroupMembers(groupId, user);

        string myRole = userGroup.Role;
        var memberDto = members.Select(m =>
        {
            bool isMe = m.UserId == user.Id;
            return m.AppUser.ToGroupMemberDto(m.Role, isMe);
        });

        var response = new GroupMemberResponseDto
        {
            GroupMembers = memberDto,
            MyRole = myRole
        };
        return response;
    }

    public async Task RemoveGroupMember(int groupId, ClaimsPrincipal claimsPrincipal, string userId)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        var userGroup = await _groupRepo.GetUserGroup(groupId, user);

        if (userGroup == null) throw new Exception("Group not found");

        var member = await _userManager.FindByIdAsync(userId);

        if (member == null) throw new Exception("User does not exist");

        // check if user is a member of the group
        var membership = await _groupRepo.GetUserGroup(groupId, member);

        if (membership == null) throw new Exception("User is not a member of this group");

        var roleInGroup = await _groupRepo.GetRoleInGroup(groupId, user);

        if (roleInGroup == "Owner" || roleInGroup == "Co-Owner")
        {
            var group = await _groupRepo.GetGroupById(groupId, user);

            // notify removed member
            string removedUserMessage = $"{user.FullName} removed you from group {group.Name}";
            await _notificationRepo.NotifyAddedRemovedMember(groupId, userId, removedUserMessage, Enums.NotificationType.UserRemovedFromGroup);
           
            // notify other members
            string message = $"{user.FullName} removed {member.FullName} from group {group.Name}";
            await _notificationRepo.NotifyGroupMembers(groupId, user.Id, message, Enums.NotificationType.UserRemovedFromGroup, null, null, userId);

            await _groupRepo.RemoveGroupMember(groupId, membership);
        }
        else
            throw new Exception("Only the group owner can delete users");
    }
}
