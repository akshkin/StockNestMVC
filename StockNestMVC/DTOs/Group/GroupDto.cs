namespace StockNestMVC.DTOs.Group;

public class GroupDto
{
    public int GroupId { get; set; }

    public string Name { get; set;  }

    public string Role {  get; set; }

    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
}
