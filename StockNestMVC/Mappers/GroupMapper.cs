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
            Role = role
        };
    }
}
