using Microsoft.AspNetCore.Identity;
using StockNestMVC.Data;

namespace StockNestMVC.Services;

public class SeedService
{
    public static async Task SeedDatabase(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        //var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedService>>();

        try
        {
            //ensure the database is ready
            logger.LogInformation("Ensuring the database is created.");

            //for dev use EnsureCreatedAsync, for prod use MigrateAsync
            await context.Database.EnsureCreatedAsync();

            string[] roles = { "Owner", "Member", "Viewer" };

            // add roles
            logger.LogInformation("Seeding roles");
            
            foreach (string role in roles)
            {
                await AddRoleAsync(roleManager, role);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while seeding the database");
        }

    }

    private static async Task AddRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}
