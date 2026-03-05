using StockNestMVC.DTOs;
using System.Security.Claims;

namespace StockNestMVC.Interfaces;

public interface ISearchService
{
    public Task<IEnumerable<SearchResultDto>> GetSearchResults(ClaimsPrincipal claimsPrincipal, string searchTerm);
}
