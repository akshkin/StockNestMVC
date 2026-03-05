namespace StockNestMVC.DTOs;

public class PaginatedResultDto<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; }
    public bool? HasNextPage { get; set; }
}
