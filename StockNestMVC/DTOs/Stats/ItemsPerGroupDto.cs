namespace StockNestMVC.DTOs.Stats;

public class ItemsPerCategoryDto
{
    public string CategoryName { get; set; }
    public int CategoryId { get; set; }
    public string GroupName { get; set; }
    public int GroupId { get; set;  }
    public int Count { get; set; }

}
