namespace StockNestMVC.Models;

// a join table for many-to-many relationship
public class UserGroup
{
    public int UserGroupId { get; set; }

    public string UserId { get; set; }
    public AppUser AppUser { get; set; }

    public int GroupId { get; set; }
    public Group Group { get; set; }

    // role of the user
    public string Role { get; set; }
}