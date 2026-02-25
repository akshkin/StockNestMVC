namespace StockNestMVC.DTOs.Group;

public class GroupMemberDto
{
    public string UserId { get; set; }
    public string FullName { get; set; }

    public string Role {  get; set; }

    public string Email { get; set; }

    public bool IsMe { get; set; }

}
