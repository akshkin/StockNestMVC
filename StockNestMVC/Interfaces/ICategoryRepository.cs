using StockNestMVC.Models;

namespace StockNestMVC.Interfaces;

public interface ICategoryRepository
{
    public Task<IEnumerable<Category>> GetCategoriesInGroup(int groupId);

    public Task<Category?> GetCategoryById(int groupId, int categoryId);

    public Task CreateCategory(Category category);

    public Task UpdateCategory(Category category);

    public Task DeleteCategory(Category caetgory);

    public Task<bool> CheckDuplicate(int groupId, string name, int? categoryId);

}
