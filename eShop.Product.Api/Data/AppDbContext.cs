using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShop.Product.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ProductTypeEntity> Types { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<FruitProductEntity> Fruits { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ProductTypeEntity>(e =>
        {
            e.HasKey(x => x.Id);
        });
        
        builder.Entity<ProductEntity>(e =>
        {
            e.UseTptMappingStrategy();
            e.ToTable("Products");

            e.HasKey(x => x.Id);
            
            e.Property(x => x.Name).HasMaxLength(128);
            e.Property(p => p.Price).HasPrecision(18, 4);
            e.Property(x => x.PricePerUnitType).HasEnumConversion();
            e.Property(x => x.UnitOfMeasure).HasEnumConversion();
        });

        builder.Entity<FruitProductEntity>(e =>
        {
            e.ToTable("Fruits");
            
            e.Property(x => x.Variety).HasMaxLength(64);
            e.Property(x => x.Color).HasMaxLength(64);
            e.Property(x => x.CountryOfOrigin).HasMaxLength(64);
            e.Property(x => x.RipenessStage).HasMaxLength(64);
            e.Property(x => x.StorageTemperature).HasMaxLength(64);
            e.Property(x => x.Grade).HasMaxLength(64);
        });
    }
}