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


    }


    DbSet<Product> Products;
    DbSet<Sold> SellEntries;
}