using StockNestMVC.DTOs.Group;
using System.Security.Claims;

namespace StockNestMVC.Interfaces;

public interface IGroupService
{
    public Task<GroupDto> CreateGroup(CreateGroupDto createGroupDto, ClaimsPrincipal claimsPrincipal);

    public Task<IEnumerable<GroupDto>> GetGroups(ClaimsPrincipal claimsPrincipal);
}
