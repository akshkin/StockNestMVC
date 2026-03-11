using Microsoft.AspNetCore.Identity;
using StockNestMVC.Exceptions;
using StockNestMVC.Interfaces;
using StockNestMVC.Models;
using System.Security.Claims;

namespace StockNestMVC.Services;

public class UserGroupService : IUserGroupService
{
        private readonly UserManager<AppUser> _userManager;
        private readonly IGroupRepository _groupRepo;

        public UserGroupService(UserManager<AppUser> userManager, IGroupRepository groupRepo)
        {
            _userManager = userManager;
            _groupRepo = groupRepo;
        }

        public async Task<(AppUser user, UserGroup membership)> ValidateMembership(
            ClaimsPrincipal principal, int groupId)
        {
            var user = await _userManager.GetUserAsync(principal);
            if (user == null)
                throw new UnauthorizedException("User not found");

            var membership = await _groupRepo.GetUserGroup(groupId, user);
            if (membership == null)
                throw new ForbiddenException("You are not a member of this group");


            return (user, membership);
        }

        public async Task<(AppUser user, UserGroup membership)> ValidateMutationOperations(
           ClaimsPrincipal principal, int groupId)
        {
            var (user, membership) = await ValidateMembership(principal, groupId);

            if (membership.Role == "Viewer")
                throw new ForbiddenException("You do not have the permission to create, edit or delete");

            return (user, membership);
        }

}
