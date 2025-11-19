using Microsoft.EntityFrameworkCore;
using Product_Management_API.Entities;

namespace Product_Management_API.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Product entity
        modelBuilder.Entity<Product>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Product>()
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(255);

        modelBuilder.Entity<Product>()
            .Property(p => p.Brand)
            .IsRequired()
            .HasMaxLength(255);

        modelBuilder.Entity<Product>()
            .Property(p => p.SKU)
            .IsRequired()
            .HasMaxLength(50);

        // Create unique constraint on SKU
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.SKU)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Product>()
            .Property(p => p.Category)
            .IsRequired();
    }
}