using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockNestMVC.Models;
using System.Reflection.Emit;

namespace StockNestMVC.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Group> Groups { get; set; }

    public DbSet<Item> Items { get; set; }

    // name is UserGroup as that is how it got saved in the database
    public DbSet<UserGroup> UserGroup { get; set; }

    public DbSet<Notification> Notifications { get; set; }

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

        builder.Entity<Notification>()
            .HasOne(n => n.Item)
            .WithMany()
            .HasForeignKey(n => n.ItemId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Notification>()
            .HasOne(n => n.Category)
            .WithMany()
            .HasForeignKey(n => n.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Notification>()
            .HasOne(n => n.Group)
            .WithMany()
            .HasForeignKey(n => n.GroupId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
