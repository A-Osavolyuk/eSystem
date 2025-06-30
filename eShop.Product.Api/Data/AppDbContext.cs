using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShop.Product.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TypeEntity> Types { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<UnitEntity> Units { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<FruitProductEntity> Fruits { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<UnitEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(128);
            e.Property(x => x.Code).HasMaxLength(6);
        });
        
        builder.Entity<CategoryEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(128);
        });
        
        builder.Entity<TypeEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(128);

            e.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId);
        });
        
        builder.Entity<ProductEntity>(e =>
        {
            e.UseTptMappingStrategy();
            e.ToTable("Products");

            e.HasKey(x => x.Id);
            
            e.Property(x => x.Name).HasMaxLength(128);
            e.Property(p => p.Price).HasPrecision(18, 4);
            e.Property(x => x.PricePerUnitType).HasEnumConversion();
            
            e.HasOne(x => x.Unit)
                .WithMany()
                .HasForeignKey(x => x.UnitId);
            
            e.HasOne(x => x.Type)
                .WithMany()
                .HasForeignKey(x => x.TypeId);
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