using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShop.Product.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TypeEntity> Types { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<UnitEntity> Units { get; set; }
    public DbSet<CurrencyEntity> Currency { get; set; }
    public DbSet<PriceTypeEntity> PriceType { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<FruitProductEntity> Fruits { get; set; }
    public DbSet<VegetableProductEntity> Vegetables { get; set; }
    public DbSet<BerryProductEntity> Berries { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<CurrencyEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(64);
            e.Property(x => x.Code).HasMaxLength(16);
            e.Property(x => x.Symbol).HasMaxLength(4);
        });
        
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
        
        builder.Entity<PriceTypeEntity>(e =>
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
            e.Property(x => x.Description).HasMaxLength(3000);
            e.Property(p => p.Price).HasPrecision(18, 4);
            
            e.HasOne(x => x.Unit)
                .WithMany()
                .HasForeignKey(x => x.UnitId);
            
            e.HasOne(x => x.Type)
                .WithMany()
                .HasForeignKey(x => x.TypeId);
            
            e.HasOne(x => x.PriceType)
                .WithMany()
                .HasForeignKey(x => x.PriceTypeId);
        });

        builder.Entity<FruitProductEntity>(e =>
        {
            e.ToTable("Fruits");
            
            e.Property(x => x.Variety).HasMaxLength(64);
            e.Property(x => x.Color).HasMaxLength(64);
            e.Property(x => x.CountryOfOrigin).HasMaxLength(64);
            e.Property(x => x.RipenessStage).HasMaxLength(64);
            e.Property(x => x.StorageTemperature).HasMaxLength(64);
            e.Property(x => x.Grade).HasMaxLength(32);
        });
        
        builder.Entity<VegetableProductEntity>(e =>
        {
            e.ToTable("Vegetables");
            
            e.Property(x => x.Variety).HasMaxLength(64);
            e.Property(x => x.Color).HasMaxLength(64);
            e.Property(x => x.CountryOfOrigin).HasMaxLength(64);
            e.Property(x => x.RipenessStage).HasMaxLength(64);
            e.Property(x => x.StorageTemperature).HasMaxLength(64);
            e.Property(x => x.Grade).HasMaxLength(32);
        });
        
        builder.Entity<BerryProductEntity>(e =>
        {
            e.ToTable("Berries");
            
            e.Property(x => x.Variety).HasMaxLength(64);
            e.Property(x => x.Color).HasMaxLength(64);
            e.Property(x => x.CountryOfOrigin).HasMaxLength(64);
            e.Property(x => x.RipenessStage).HasMaxLength(64);
            e.Property(x => x.StorageTemperature).HasMaxLength(64);
            e.Property(x => x.Grade).HasMaxLength(32);
        });
    }
}