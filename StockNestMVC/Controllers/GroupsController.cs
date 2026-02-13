using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockNestMVC.DTOs.Group;
using StockNestMVC.Interfaces;
using StockNestMVC.Models;
using System.Security.Claims;

namespace StockNestMVC.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IGroupRepository _groupRepo;


    public GroupsController(UserManager<AppUser> userManager, IGroupRepository groupRepo)
    {
        _userManager = userManager;
        _groupRepo = groupRepo;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateGroup(CreateGroupDto createGroupDto)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine("UserId claim: " + id);
            Console.WriteLine(User.Identity.IsAuthenticated);
            Console.WriteLine(user.Email);

            if (user == null) return BadRequest("No user found");

            var group = await _groupRepo.CreateGroup(createGroupDto, user);

            if (group == null) return BadRequest("There was a problem creating the group");

            return Ok(group);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        

    }

    [HttpPost("edit/{id}")]
    public async Task<IActionResult> UpdateGroup(int id, CreateGroupDto updateGroupDto)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
           
            if (user == null) return BadRequest("No user found");

            var updatedGroup = await _groupRepo.UpdateGroup(id, updateGroupDto, user);

            return Ok(updatedGroup);
        }
        catch (Exception ex) 
        {
            return BadRequest(ex.Message);
        }
    }
}
