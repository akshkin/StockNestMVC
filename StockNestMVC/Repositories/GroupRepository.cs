using Microsoft.EntityFrameworkCore;
using StockNestMVC.Data;
using StockNestMVC.DTOs.Group;
using StockNestMVC.Interfaces;
using StockNestMVC.Mappers;
using StockNestMVC.Models;
using System.Data;
using Group = StockNestMVC.Models.Group;

namespace StockNestMVC.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly ApplicationDbContext _context;

    public GroupRepository(ApplicationDbContext context)
    {
        _context = context;
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

        var newGroup = new Models.Group { Name = createGroupDto.Name };

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

        return newGroup.ToGroupDto(userGroup.Role);
    }

    public async Task<GroupDto?> UpdateGroup(int id, CreateGroupDto updateGroupDto, AppUser user)
    {
        var existingGroup = await _context.UserGroup.Include(ug => ug.Group)
            .Where(ug => ug.UserId == user.Id && ug.GroupId == id)
            .Select(ug => ug.Group)
            .FirstOrDefaultAsync(ug => ug.GroupId == id);

        var roleInGroup = await GetRoleInGroup(id, user);
        //var existingGroup = await GetGroupById(id, user);

        if (existingGroup == null)
        {
            throw new Exception($"Group with id {id} not found");
        }

        // find duplicate but allow to update the current group with the
        // same name in case request is sent with the same name
        var duplicate = await _context.UserGroup
            .Include(ug => ug.Group)
            .AnyAsync(g => 
                g.UserId == user.Id && 
                g.Group.Name == updateGroupDto.Name && 
                g.GroupId != id);

        if (duplicate)
        {
            throw new Exception("Group with the same name already exists");
        }

        var role = await GetRoleInGroup(id, user);

        existingGroup.Name = updateGroupDto.Name;

        await _context.SaveChangesAsync();

        return existingGroup.ToGroupDto(role);

    }

    public async Task<IEnumerable<GroupDto>> GetAllUserGroups(AppUser user)
    {
        var userGroups = await _context.UserGroup
            .Include(ug => ug.Group)
            .Where(ug => ug.UserId == user.Id)
            .ToListAsync();

        return userGroups.Select(ug => ug.Group.ToGroupDto(ug.Role));
    }

    public async Task<GroupDto?> GetGroupById(int id, AppUser user)
    {
        var existingGroup = await _context.UserGroup.Include(ug => ug.Group)
            .Where(ug => ug.UserId == user.Id && ug.GroupId == id)
            .Select(ug => ug.Group)
            .FirstOrDefaultAsync(ug => ug.GroupId == id);

        var roleInGroup = await GetRoleInGroup(id, user);

        if (existingGroup == null) return null;

        return existingGroup.ToGroupDto(roleInGroup);
    }

    public async Task<GroupDto?> DeleteGroup(int id, AppUser user)
    {
        // since the get by id method returns a dto, it cannot be reused here
        var existingGroup = await _context.UserGroup.Include(ug => ug.Group)
             .Where(ug => ug.UserId == user.Id && ug.GroupId == id)
             .Select(ug => ug.Group)
             .FirstOrDefaultAsync(ug => ug.GroupId == id);

        if (existingGroup == null) return null;

        // check if user is the owner
        var role = await GetRoleInGroup(id, user);

        if (role == "Owner" || role == "Co-Owner")
        {
            _context.Remove(existingGroup);
            await _context.SaveChangesAsync();
            return existingGroup.ToGroupDto(role);
        }
        else 
            throw new Exception("Only the group owner can delete the group");

    }

    public async Task InviteUser(int groupId, AppUser invitedUser, string role,  AppUser user)
    {
        // Check if group exists
        var group = await _context.Groups.FindAsync(groupId);
        if (group == null)
            throw new Exception("Group not found");

        //  Check if already in group
        var alreadyMember = await _context.UserGroup
            .AnyAsync(ug => ug.GroupId == groupId && ug.UserId == invitedUser.Id);

        if (alreadyMember)
            throw new Exception("User is already a member of this group");

        // Check if inviter is Owner
        var inviterMember = await GetRoleInGroup(groupId, user);

        if (inviterMember == "Owner" || inviterMember == "Co-Owner")
        {
            // Add to group
            var newMembership = new UserGroup
            {
                UserId = invitedUser.Id,
                GroupId = groupId,
                Role = role
            };

            await _context.UserGroup.AddAsync(newMembership);
            await _context.SaveChangesAsync();
        }
        else 
            throw new Exception("Only the group owner can invite users");     

    }

    public async Task<string> GetRoleInGroup (int id, AppUser user)
    {
        // check if user is the owner
        var appUser = await _context.UserGroup
           .FirstOrDefaultAsync(ug => ug.GroupId == id && ug.UserId == user.Id);

        if (appUser == null)
            return null;

        return appUser.Role;
    }

    public async Task<GroupMemberResponseDto> GetGroupMembers(int groupId, AppUser user)
    {
        var userGroup = await _context.UserGroup
          .FirstOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == user.Id);

        if (userGroup == null) return null;

        var members = await _context.UserGroup.Include(ug => ug.AppUser).Where(ug => ug.GroupId == groupId).ToListAsync();

        string myRole = userGroup.Role;
        var memberDto =  members.Select(m => {
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

    public async Task RemoveGroupMember(int groupId, AppUser owner, AppUser member)
    {
        var group = await _context.Groups.FindAsync(groupId);
        if (group == null)
            throw new Exception("Group not found");

        //  Check if member exists in group
        var groupMember = await _context.UserGroup
            .FirstOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == member.Id);

        if (groupMember == null)
            throw new Exception("User is not a member of this group");

        // Check if inviter is Owner
        var inviterMember = await GetRoleInGroup(groupId, owner);

        if (inviterMember == "Owner" || inviterMember == "Co-Owner")
        {
            _context.UserGroup.Remove(groupMember);
            await _context.SaveChangesAsync();
        }
        else 
            throw new Exception("Only the group owner can delete users");


    }
}
