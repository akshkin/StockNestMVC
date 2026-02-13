using Microsoft.EntityFrameworkCore;
using StockNestMVC.Data;
using StockNestMVC.DTOs.Group;
using StockNestMVC.Interfaces;
using StockNestMVC.Mappers;
using StockNestMVC.Models;

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

    public async Task<GroupDto?> UpdateGroup(int id, CreateGroupDto updateGroupDto, AppUser user)
    {
        var existingGroup = await GetGroupById(id, user);

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

        existingGroup.Name = updateGroupDto.Name;

        await _context.SaveChangesAsync();

        return existingGroup;

    }

    public async Task<IEnumerable<GroupDto>> GetAllUserGroups(AppUser user)
    {
        var userGroups = await _context.UserGroup
            .Include(ug => ug.Group)
            .Where(ug => ug.UserId == user.Id)
            .ToListAsync();

        return userGroups.Select(ug => ug.Group.ToGroupDto());
    }

    public async Task<GroupDto?> GetGroupById(int id, AppUser user)
    {
        var existingGroup = await _context.UserGroup.Include(ug => ug.Group)
            .Where(ug => ug.UserId == user.Id && ug.GroupId == id)
            .Select(ug => ug.Group)
            .FirstOrDefaultAsync(ug => ug.GroupId == id);

        if (existingGroup == null) return null;

        return existingGroup.ToGroupDto();

    }

    public async Task<GroupDto?> DeleteGroup(int id, AppUser user)
    {
        var existingGroup = await GetGroupById(id, user);

        if (existingGroup == null) return null;

        _context.Remove(existingGroup);
        await _context.SaveChangesAsync();
        return existingGroup;
    }
}
