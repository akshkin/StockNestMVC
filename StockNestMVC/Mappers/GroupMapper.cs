using StockNestMVC.DTOs.Group;
using StockNestMVC.Models;

namespace StockNestMVC.Mappers;

public static class GroupMapper
{
    public static GroupDto ToGroupDto(this Group group, string role)
    {
        return new GroupDto
        {
            GroupId = group.GroupId,
            Name = group.Name,
            Role = role,
            CreatedAt = group.CreatedAt,
            CreatedBy = group.CreatedBy,
            UpdatedAt = group.UpdatedAt,
            UpdatedBy = group.UpdatedBy,
        };
    }

    public static GroupMemberDto ToGroupMemberDto(this AppUser user, string role, bool isMe)
    {
        return new GroupMemberDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Role = role,
            Email = user.Email,
            IsMe = isMe
        };
    }
}
