using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class ScopeConfiguration : IEntityTypeConfiguration<ScopeEntity>
{
    public void Configure(EntityTypeBuilder<ScopeEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.Value).HasMaxLength(100);
    }
}