using Microsoft.EntityFrameworkCore;
using webapi.Models;

namespace webapi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Dish> Dishes { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalPayment)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Dish>()
            .HasMany(d => d.Categories)
            .WithMany(c => c.Dishes)
            .UsingEntity<Dictionary<string, object>>(
                "DishCategories",
                j => j.HasOne<Category>().WithMany().HasForeignKey("CategoryId"),
                j => j.HasOne<Dish>().WithMany().HasForeignKey("DishId")
    );


    }
}