using StockNestMVC.Data;
using StockNestMVC.Interfaces;

namespace StockNestMVC.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly ApplicationDbContext _context;

    public ItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }
}
