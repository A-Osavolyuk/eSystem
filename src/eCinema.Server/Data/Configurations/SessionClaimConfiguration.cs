using eCinema.Server.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCinema.Server.Data.Configurations;

public class SessionClaimConfiguration : IEntityTypeConfiguration<SessionClaimEntity>
{
    public void Configure(EntityTypeBuilder<SessionClaimEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Type).HasMaxLength(100);
        builder.Property(e => e.Value).HasMaxLength(1000);
    }
}