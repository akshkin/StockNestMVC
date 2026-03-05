using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockNestMVC.DTOs.Group;
using StockNestMVC.Interfaces;

namespace StockNestMVC.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupsController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGroups()
    {
        try
        {
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
            var group = await _groupService.DeleteGroup(id, User);

            if (group == null) return NotFound($"Group with id {id} not found");

            return Ok("Successfully deleted group");
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{groupId}/invite")]
    public async Task<IActionResult> InviteUser(int groupId, InviterDto dto)
    {
        try
        {
            await _groupService.InviteGroupMember(groupId, User, dto.Email, dto.Role);

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
        try
        {
            var members = await _groupService.GetGroupMembers(groupId, User);

            return Ok(members);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{groupId}/deleteMember/{userId}")]
    public async Task<IActionResult> RemoveGroupMember(int groupId, string userId)
    {
        try
        {
            await _groupService.RemoveGroupMember(groupId, User, userId);

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
