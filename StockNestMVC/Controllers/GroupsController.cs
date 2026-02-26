using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockNestMVC.DTOs.Group;
using StockNestMVC.Interfaces;
using StockNestMVC.Models;

namespace StockNestMVC.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IGroupRepository _groupRepo;
    private readonly IGroupService _groupService;

    public GroupsController(UserManager<AppUser> userManager, IGroupRepository groupRepo, IGroupService groupService)
    {
        _userManager = userManager;
        _groupRepo = groupRepo;
        _groupService = groupService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGroups()
    {
        try
        {
            var user = await IsUserExists();

            if (user == null) return BadRequest("No user found");
            //var groups = await _groupRepo.GetAllUserGroups(user);
            var groups = await _groupService.GetGroups(User);
            return Ok(groups);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGroupById(int id)
    {
        try
        {
            var user = await IsUserExists();

            if (user == null) return BadRequest("No user found");

            var group = await _groupService.GetGroupById(id, User);

            if (group == null)
            {
                return NotFound($"Group with id {id} not found");
            }
            return Ok(group);
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateGroup(CreateGroupDto createGroupDto)
    {
        try
        {
            var user = await IsUserExists();

            if (user == null) return BadRequest("No user found");

            var group = await _groupService.CreateGroup(createGroupDto, User);

            if (group == null) return BadRequest("There was a problem creating the group");

            return Ok(group);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        

    }

    [HttpPost("{id}/edit")]
    public async Task<IActionResult> UpdateGroup(int id, CreateGroupDto updateGroupDto)
    {
        try
        {
            var user = await IsUserExists();
           
            if (user == null) return BadRequest("No user found");

            var updatedGroup = await _groupService.UpdateGroup(id, updateGroupDto, User);

            if (updatedGroup == null) return NotFound($"Group with id {id} not found");

            return Ok(updatedGroup);
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/delete")]
    public async Task<IActionResult> DeleteGroup(int id) 
    {
        try
        {
            var user = await IsUserExists();

            if (user == null) return BadRequest("No user found");

            var group = await _groupService.DeleteGroup(id, User);

            if (group == null) return NotFound($"Group with id {id} not found");

            return Ok("Successfully deleted group");
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }

    private async Task<AppUser?> IsUserExists()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null) return null;

        return user;
    }

    [HttpPost("{groupId}/invite")]
    public async Task<IActionResult> InviteUser(int groupId, InviterDto dto)
    {
        try
        {
            var inviter = await IsUserExists();
            if (inviter == null) return NotFound("User not found");

            var invitedUser = await _userManager.FindByEmailAsync(dto.Email);
            if (invitedUser == null) return NotFound($"Invitee with email address {dto.Email} does not exist in our database.");

            await _groupRepo.InviteUser(groupId, invitedUser, dto.Role, inviter);

            return Ok(new { message = "User invited successfully" });
        } 
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{groupId}/members")]
    public async Task<IActionResult> GetGroupMembers(int groupId)
    {
        var user = await IsUserExists();

        if (user == null) return NotFound("No user found");

        var members = await _groupRepo.GetGroupMembers(groupId, user);

        return Ok(members);
    }

    [HttpPost("{groupId}/deleteMember/{userId}")]
    public async Task<IActionResult> RemoveGroupMember(int groupId, string userId)
    {
        try
        {
            var user = await IsUserExists();

            if (user == null) return NotFound("No user found");

            var member = await _userManager.FindByIdAsync(userId);

            if (member == null) return NotFound("Member does not exist");

            await _groupRepo.RemoveGroupMember(groupId, user, member);

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
