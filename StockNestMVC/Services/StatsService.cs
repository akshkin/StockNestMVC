using Microsoft.AspNetCore.Identity;
using StockNestMVC.DTOs.Stats;
using StockNestMVC.Exceptions;
using StockNestMVC.Interfaces;
using StockNestMVC.Models;
using System.Security.Claims;

namespace StockNestMVC.Services;

public class StatsService : IStatsService
{
    private readonly IStatsRepository _statsRepo;
    private readonly UserManager<AppUser> _userManager;
    private readonly INotificationService _notificationService;

    public StatsService(
        IStatsRepository statsRepo,
        UserManager<AppUser> userManager,
        INotificationService notificationService)
        
    {
        _statsRepo = statsRepo;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    public async Task<StatsDto> GetGroupStats(ClaimsPrincipal principal)
    {
        var user = await _userManager.GetUserAsync(principal);

        if (user == null) throw new UnauthorizedException("User not found");

        var totalGroups = await _statsRepo.CountGroups(user);
        var totalCategories = await _statsRepo.CountCategories(user);
        var totalItems = await _statsRepo.CountItems(user);

        var userCreatedItems = await _statsRepo.CountItemsCreatedByUser(user);
        var userUpdatedItems = await _statsRepo.CountItemsUpdatedByUser(user);

        var itemsPerGroup = await _statsRepo.GetItemsPerGroup(user);
        var topCategories = await _statsRepo.GetTopCategories(user);

        var latestNotifications = await _notificationService.GetLatestNotifications(principal);

        return new StatsDto
        {
            TotalGroups = totalGroups,
            TotalCategories = totalCategories,
            TotalItems = totalItems,
            UserCreatedItems = userCreatedItems,
            UserUpdatedItems = userUpdatedItems,
            ItemsPerGroup = itemsPerGroup,
            TopCategories = topCategories,
            LatestNotifications = latestNotifications
        };
    }
    
}
