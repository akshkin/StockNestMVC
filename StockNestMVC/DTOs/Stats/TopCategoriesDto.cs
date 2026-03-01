namespace StockNestMVC.DTOs.Stats;

public class TopCategoriesDto
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; }

    public int GroupId { get; set; }

    public string GroupName { get; set; }

    public int ItemsCount { get; set; }
}
