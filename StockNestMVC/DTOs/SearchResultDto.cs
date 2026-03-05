namespace StockNestMVC.DTOs;

public class SearchResultDto
{
    public string Type { get; set; }

    public int GroupId { get; set; }
    
    public string Name { get; set; }

    public int? CategoryId { get; set; }

    public int? ItemId { get; set; }
}
