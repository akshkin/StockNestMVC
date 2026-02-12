using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockNestMVC.Models;

namespace StockNestMVC.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Group> Groups { get; set; }

    public DbSet<Item> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserGroup>()
            .HasKey(ug => new { ug.UserId, ug.GroupId });

        builder.Entity<UserGroup>()
            .HasOne(ug => ug.AppUser)
            .WithMany(u => u.UserGroups)
            .HasForeignKey(ug => ug.UserId);

        builder.Entity<UserGroup>()
            .HasOne(ug => ug.Group)
            .WithMany(ug => ug.UserGroups)
            .HasForeignKey(ug => ug.GroupId);
    }
}
