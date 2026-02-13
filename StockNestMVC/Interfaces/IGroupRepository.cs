using StockNestMVC.DTOs.Group;
using StockNestMVC.Models;

namespace StockNestMVC.Interfaces;

public interface IGroupRepository
{
    public Task<IEnumerable<GroupDto>> GetAllUserGroups(AppUser user);

    public Task<GroupDto?> GetGroupById(int id, AppUser user);

    public Task<GroupDto> CreateGroup(CreateGroupDto createGroupDto, AppUser user);

    public Task<GroupDto?> UpdateGroup(int id, CreateGroupDto updateGroupDto, AppUser user);
}
