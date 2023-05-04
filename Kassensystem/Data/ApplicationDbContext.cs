using Microsoft.EntityFrameworkCore;

namespace Kassensystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {



    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasMany(p => p.SellEntries)
            .WithOne(s => s.Item);

        modelBuilder.Entity<User>()
            .HasMany(u => u.SellEntries)
            .WithOne(e => e.SoldBy);

    }


    public DbSet<Product> Products;
    public DbSet<Sold> SellEntries;
    public DbSet<User> Users;
}