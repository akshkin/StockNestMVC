using Microsoft.EntityFrameworkCore;
using StockNestMVC.Data;
using StockNestMVC.Interfaces;
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

    public async Task CreateGroup(Group group)
    {
        await _context.Groups.AddAsync(group);
        await _context.SaveChangesAsync();
    }

    public async Task CreateUserGroup(UserGroup membership)
    {
        await _context.UserGroup.AddAsync(membership);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateGroup(Group group)
    {
        _context.Groups.Update(group);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<UserGroup>> GetAllUserGroups(AppUser user)
    {
        var userGroups = await _context.UserGroup
            .Include(ug => ug.Group)
            .Where(ug => ug.UserId == user.Id)
            .ToListAsync();

        return userGroups;
    }

    public async Task<Group?> GetGroupById(int id, AppUser user)
    {
        var existingGroup = await _context.UserGroup.Include(ug => ug.Group)
            .Where(ug => ug.UserId == user.Id && ug.GroupId == id)
            .Select(ug => ug.Group)
            .FirstOrDefaultAsync(ug => ug.GroupId == id);

        if (existingGroup == null) return null;
        return existingGroup;
    }

    public async Task DeleteGroup(Group group)
    {      
        _context.Remove(group);
        await _context.SaveChangesAsync();
    }

    public async Task InviteUser(UserGroup membership)
    {
        await _context.UserGroup.AddAsync(membership);
        await _context.SaveChangesAsync();
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

    public async Task<IEnumerable<UserGroup>> GetGroupMembers(int groupId, AppUser user)
    {
        var members = await _context.UserGroup.Include(ug => ug.AppUser).Where(ug => ug.GroupId == groupId).ToListAsync();

        return members;
    }

    public async Task RemoveGroupMember(int groupId, UserGroup membership)
    {        
        _context.UserGroup.Remove(membership);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CheckDuplicateGroup(AppUser user, string groupName, int? groupId)
    {
        bool duplicate;

        if (groupId != null)
        {
            duplicate = await _context.UserGroup
                .Include(ug => ug.Group)
                .AnyAsync(g =>
                    g.UserId == user.Id &&
                    g.Group.Name == groupName &&
                    g.GroupId != groupId);
        }
        else
        {
            duplicate = await _context.UserGroup
               .Include(ug => ug.Group)
               .AnyAsync(g => g.UserId == user.Id && g.Group.Name == groupName);
        }

        return duplicate;
    }

    public async Task<bool> CheckIfMemberInGroup(int groupId, AppUser user)
    {
        var alreadyMember = await _context.UserGroup
            .AnyAsync(ug => ug.GroupId == groupId && ug.UserId == user.Id);

        return alreadyMember;
    }

    public async Task<UserGroup?> GetUserGroup(int groupId, AppUser user)
    {
        var userGroup = await _context.UserGroup
          .FirstOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == user.Id);

        if (userGroup == null) return null;

        return userGroup;
    }
}
