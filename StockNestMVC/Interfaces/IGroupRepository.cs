using StockNestMVC.DTOs.Group;
using StockNestMVC.Models;

namespace StockNestMVC.Interfaces;

public interface IGroupRepository
{
    public Task<IEnumerable<UserGroup>> GetAllUserGroups(AppUser user);

    public Task<Group?> GetGroupById(int id, AppUser user);

    public Task CreateGroup(Group group);

    public Task CreateUserGroup(UserGroup membership);

    public Task UpdateGroup(Group group);

    public Task DeleteGroup(Group group);

    public Task InviteUser(UserGroup membership);

    public Task<string> GetRoleInGroup(int id, AppUser user);

    public Task<UserGroup?> GetUserGroup(int groupId, AppUser user);

    public Task<IEnumerable<UserGroup>> GetGroupMembers(int groupId, AppUser user);

    public Task RemoveGroupMember(int groupId, UserGroup membership);

    public Task<bool> CheckDuplicateGroup(AppUser user, string groupName, int? groupId);

    public Task<bool> CheckIfMemberInGroup(int groupId, AppUser user);
}
