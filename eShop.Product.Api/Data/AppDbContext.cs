using eShop.Domain.Enums;
using eShop.Product.Api.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShop.Product.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ProductEntity> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ProductEntity>(e =>
        {
            e.UseTptMappingStrategy();

            e.HasKey(x => x.Id);
            
            e.Property(x => x.Name).HasMaxLength(128);
            e.Property(p => p.Price).HasPrecision(18, 4);
            e.Property(x => x.ProductType).HasEnumConversion();
            e.Property(x => x.PricePerUnitType).HasEnumConversion();
            e.Property(x => x.UnitOfMeasure).HasEnumConversion();
        });
    }
}