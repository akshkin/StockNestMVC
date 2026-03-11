using StockNestMVC.Models;
using System.Security.Claims;

namespace StockNestMVC.Interfaces;

public interface IUserGroupService
{
    public Task<(AppUser user, UserGroup membership)> ValidateMembership(
            ClaimsPrincipal principal, int groupId);

    public Task<(AppUser user, UserGroup membership)> ValidateMutationOperations(
           ClaimsPrincipal principal, int groupId);
}
