namespace StockNestMVC.Interfaces;

public interface ISeedService
{
    public Task SeedDatabase(IServiceProvider serviceProvider);
}
