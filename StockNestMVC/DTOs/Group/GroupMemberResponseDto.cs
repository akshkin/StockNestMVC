namespace StockNestMVC.DTOs.Group;

public class GroupMemberResponseDto
{
    public IEnumerable<GroupMemberDto> GroupMembers { get; set; }

    public string MyRole { get; set; }
}
