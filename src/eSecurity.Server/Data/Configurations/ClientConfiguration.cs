using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Data.Conversion;
using eSystem.Core.Security.Authentication.OpenIdConnect.BackchannelAuthentication;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
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
        builder.Property(x => x.ClientType)
            .HasConversion<EnumValueConverter<ClientType>>();
        
        builder.Property(x => x.AccessTokenType)
            .HasConversion<EnumValueConverter<AccessTokenType>>();
        
        builder.Property(x => x.SubjectType)
            .HasConversion<EnumValueConverter<SubjectType>>();
        
        builder.Property(x => x.NotificationDeliveryMode)
            .HasConversion<EnumValueConverter<NotificationDeliveryMode>>();
        
        builder.Property(x => x.RefreshTokenLifetime).HasConversion(ValueConverters.NullableTimeSpan);
        builder.Property(x => x.AccessTokenLifetime).HasConversion(ValueConverters.NullableTimeSpan);
        builder.Property(x => x.IdTokenLifetime).HasConversion(ValueConverters.NullableTimeSpan);
        builder.Property(x => x.LoginTokenLifetime).HasConversion(ValueConverters.NullableTimeSpan);
        builder.Property(x => x.LogoutTokenLifetime).HasConversion(ValueConverters.NullableTimeSpan);
    }
}