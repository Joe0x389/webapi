using Microsoft.EntityFrameworkCore;
using webapi.Models;

namespace webapi.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Dish> Dishes { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<User> Users { get; set; } 
}