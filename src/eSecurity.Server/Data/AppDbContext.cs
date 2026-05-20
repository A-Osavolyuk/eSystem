using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace eSecurity.Server.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<SessionEntity> Sessions { get; set; }
    public DbSet<SessionPropertiesEntity> SessionProperties { get; set; }
    public DbSet<SessionTokenEntity> SessionTokens { get; set; }
    public DbSet<SessionClaimEntity> SessionClaims { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("public");
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}