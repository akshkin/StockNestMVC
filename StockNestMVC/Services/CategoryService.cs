using Microsoft.AspNetCore.Identity;
using StockNestMVC.DTOs.Category;
using StockNestMVC.Interfaces;
using StockNestMVC.Mappers;
using StockNestMVC.Models;
using System.Security.Claims;

namespace StockNestMVC.Services;

public class CategoryService : ICategoryService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IGroupRepository _groupRepo;
    private readonly INotificationRepository _notificationRepo;

    public CategoryService(UserManager<AppUser> userManager, ICategoryRepository categoryRepo, IGroupRepository groupRepo, INotificationRepository notificationRepo)
    {
        _userManager = userManager;
        _categoryRepo = categoryRepo;
        _groupRepo = groupRepo;
        _notificationRepo = notificationRepo;
    }

    public async Task<CategoryDto> CreateCategory(int groupId, ClaimsPrincipal claimsPrincipal, CreateCategoryDto createCategoryDto)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        var userRole = await _groupRepo.GetRoleInGroup(groupId, user);

        if (userRole == "Viewer") throw new Exception("You do not have permission to create categories");

        var duplicate = await _categoryRepo.CheckDuplicate(groupId, createCategoryDto.Name, null);

        if (duplicate)
            throw new Exception("A category with this name already exists in the group");

        var newCategory = new Category
        {
            Name = createCategoryDto.Name,
            GroupId = groupId,
            CreatedBy = user.Id
        };
        await _categoryRepo.CreateCategory(newCategory);

        var group = await _groupRepo.GetGroupById(groupId, user);

        string message = $"{user.FullName} created a new category {newCategory.Name} in group {group.Name}";

        await _notificationRepo.NotifyGroupMembers(groupId, user.Id, message, Enums.NotificationType.CategoryCreated, newCategory.CategoryId);

        return newCategory.ToCategoryDto(user.FullName, null);
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesInGroup(int groupId, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        //check if user belongs to this group
        var membership = await _groupRepo.GetUserGroup(groupId, user);

        if (membership == null) throw new Exception("You are not a member of this group");

        var categories = await _categoryRepo.GetCategoriesInGroup(groupId);

        var categoryDtos = new List<CategoryDto>();

        foreach(Category category in categories)
        {
            var (creatorName, updatorName) = await GetCreatorUpdatorNames(category, user);
            categoryDtos.Add(category.ToCategoryDto(creatorName, updatorName));
        }

        return categoryDtos;
    }

    public async Task<CategoryDto?> GetCategoryById(int groupId, int categoryId, ClaimsPrincipal claimsPrincipal)
    {
        var (user, membership) = await ValidateMembership(groupId, claimsPrincipal);

        var category = await _categoryRepo.GetCategoryById(groupId, categoryId);

        if (category == null) throw new Exception("Category was not found");

        var (creatorName, updatorName) = await GetCreatorUpdatorNames(category, user);

        return category.ToCategoryDto(creatorName, updatorName);
    }

    public async Task<CategoryDto?> UpdateCategory(int groupId, int categoryId, ClaimsPrincipal claimsPrincipal, CreateCategoryDto updateCategoryDto)
    {
        var (user, membership) = await ValidateMembership(groupId, claimsPrincipal);

        var category = await _categoryRepo.GetCategoryById(groupId, categoryId);

        if (category == null) throw new Exception("Category was not found");

        var userRole = await _groupRepo.GetRoleInGroup(groupId, user);

        if (userRole == "Viewer") throw new Exception("You do not have permission to update categories");

        var duplicate = await _categoryRepo.CheckDuplicate(groupId, updateCategoryDto.Name, categoryId);

        if (duplicate) throw new Exception("A category with this name already exists in the group");

        category.Name = updateCategoryDto.Name;
        category.UpdatedAt = DateTime.UtcNow;
        category.UpdatedBy = user.Id;

        await _categoryRepo.UpdateCategory(category);

        var group = await _groupRepo.GetGroupById(groupId, user);

        string message = $"{user.FullName} updated category {category.Name} in group {group.Name}";

        await _notificationRepo.NotifyGroupMembers(groupId, user.Id, message, Enums.NotificationType.CategoryUpdated, categoryId);

        return category.ToCategoryDto(category.CreatedBy, user.FullName);
    }

    public async Task<CategoryDto?> DeleteCategory(int groupId, int categoryId, ClaimsPrincipal claimsPrincipal)
    {
        var (user, membership) = await ValidateMembership(groupId, claimsPrincipal);

        var category = await _categoryRepo.GetCategoryById(groupId, categoryId);

        if (category == null) throw new Exception("Category was not found");

        var userRole = await _groupRepo.GetRoleInGroup(groupId, user);

        if (userRole == "Viewer") throw new Exception("You do not have permission to delete categories");

        await _categoryRepo.DeleteCategory(category);

        var group = await _groupRepo.GetGroupById(groupId, user);

        string message = $"{user.FullName} deleted category {category.Name} from group {group.Name}";

        await _notificationRepo.NotifyGroupMembers(groupId, user.Id, message, Enums.NotificationType.CategoryDeleted, categoryId);

        return category.ToCategoryDto(category.CreatedBy, category.UpdatedBy);
    }

    private async Task<(AppUser user, UserGroup membership)> ValidateMembership(int groupId, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user == null) throw new Exception("User not found");

        var membership = await _groupRepo.GetUserGroup(groupId, user);

        if (membership == null) throw new Exception("You are not a member of this group");

        return (user, membership);
    }

    private async Task<(string? createdBy, string? updatedBy)> GetCreatorUpdatorNames(Category category, AppUser currentUser)
    {
        string? creatorName = null;
        if (category.CreatedBy != null)
        {
            var creator = await _userManager.FindByIdAsync(category.CreatedBy);
            creatorName = category.CreatedBy == currentUser.Id ? "You" : creator?.FullName;
        }

        string? updaterName = null;
        if (category.UpdatedBy != null)
        {
            var updator = await _userManager.FindByIdAsync(category.UpdatedBy);
            updaterName = category.UpdatedBy == currentUser.Id ? "You" : updator?.FullName;
        }
        return (creatorName, updaterName);
    }
}
