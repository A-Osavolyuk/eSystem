using eCinema.Server.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace eCinema.Server.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<SessionEntity> Sessions { get; set; }
    public DbSet<SessionTokenEntity> Tokens { get; set; }
    public DbSet<SessionClaimEntity> Claims { get; set; }
    public DbSet<SessionPropertiesEntity> Properties { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("public");
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}