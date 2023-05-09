using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Kassensystem.Data.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasMany(p => p.SellEntries)
            .WithOne(s => s.Item);

        modelBuilder.Entity<Product>()
            .HasOne(e => e.Image)
            .WithOne(e => e.Product)
            .HasForeignKey<ProductImage>();

        modelBuilder.Entity<User>()
            .HasMany(u => u.SellEntries)
            .WithOne(e => e.SoldBy);
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Cart)
            .WithOne(e => e.User);

    }


    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImage { get; set; }
    public DbSet<Sold> SellEntries { get; set; }
    public DbSet<User> Users { get; set; }
}