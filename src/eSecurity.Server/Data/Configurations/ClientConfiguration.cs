using eSecurity.Server.Data.Entities;
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
        builder.Property(x => x.LogoUri).HasMaxLength(100);
        builder.Property(x => x.ClientUri).HasMaxLength(100);
        builder.Property(x => x.ClientType).HasConversion<string>();
        builder.Property(x => x.AccessTokenType).HasConversion<string>();
        builder.Property(x => x.SubjectType).HasConversion<string>();
        builder.Property(x => x.RefreshTokenLifetime).HasConversion(ValueConverters.NullableTimeSpan);
        builder.Property(x => x.AccessTokenLifetime).HasConversion(ValueConverters.NullableTimeSpan);
        builder.Property(x => x.IdTokenLifetime).HasConversion(ValueConverters.NullableTimeSpan);
        builder.Property(x => x.LoginTokenLifetime).HasConversion(ValueConverters.NullableTimeSpan);
        builder.Property(x => x.LogoutTokenLifetime).HasConversion(ValueConverters.NullableTimeSpan);
    }
}