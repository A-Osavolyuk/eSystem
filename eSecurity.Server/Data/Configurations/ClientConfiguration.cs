using eSecurity.Server.Data.Entities;
using eSystem.Core.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class ClientConfiguration : IEntityTypeConfiguration<ClientEntity>
{
    public void Configure(EntityTypeBuilder<ClientEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Secret).HasMaxLength(200);
        builder.Property(x => x.SectorIdentifierUri).HasMaxLength(200);
        builder.Property(x => x.Name).HasMaxLength(64);
        builder.Property(x => x.Audience).HasMaxLength(64);
        builder.Property(x => x.LogoUri).HasMaxLength(100);
        builder.Property(x => x.ClientUri).HasMaxLength(100);
        builder.Property(x => x.ClientType).HasEnumConversion();
        builder.Property(x => x.AccessTokenType).HasEnumConversion();
        builder.Property(x => x.SubjectType).HasEnumConversion();
        builder.Property(x => x.RefreshTokenLifetime).HasConversion(
            time => time.Ticks,
            ticks => TimeSpan.FromTicks(ticks));
    }
}