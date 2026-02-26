using StockNestMVC.DTOs.Group;
using System.Security.Claims;

namespace StockNestMVC.Interfaces;

public interface IGroupService
{
    public Task<GroupDto> CreateGroup(CreateGroupDto createGroupDto, ClaimsPrincipal claimsPrincipal);

    public Task<IEnumerable<GroupDto>> GetGroups(ClaimsPrincipal claimsPrincipal);

    public Task<GroupDto?> UpdateGroup(int groupId, CreateGroupDto updateGroupDto, ClaimsPrincipal claimsPrincipal);

    public Task<GroupDto?> GetGroupById(int groupId, ClaimsPrincipal claimsPrincipal);

    public Task<GroupDto?> DeleteGroup(int groupId, ClaimsPrincipal claimsPrincipal); 
}
