using Microsoft.AspNetCore.Identity;
using StockNestMVC.DTOs;
using StockNestMVC.Interfaces;
using StockNestMVC.Models;
using System.Security.Claims;

namespace StockNestMVC.Services;

public class SearchService : ISearchService
{
    private readonly IGroupRepository _groupRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IItemRepository _itemRepo;
    private readonly UserManager<AppUser> _userManager;

    public SearchService(IGroupRepository groupRepo, 
        ICategoryRepository categoryRepo, 
        IItemRepository itemRepo, 
        UserManager<AppUser> userManager
    )
    {
        _groupRepo = groupRepo;
        _categoryRepo = categoryRepo;
        _itemRepo = itemRepo;
        _userManager = userManager;
    }

    public async Task<IEnumerable<SearchResultDto>> GetSearchResults(ClaimsPrincipal claimsPrincipal, string searchTerm)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        var groups = await _groupRepo.GetSearchResult(user, searchTerm);
        var categories = await _categoryRepo.GetSearchResult(user,searchTerm);
        var items = await _itemRepo.GetSearchResult(user, searchTerm);

        return groups.Concat(categories).Concat(items);
    }
}
