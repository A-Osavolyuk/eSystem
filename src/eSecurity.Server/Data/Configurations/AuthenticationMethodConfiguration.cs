using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class AuthenticationMethodConfiguration : IEntityTypeConfiguration<AuthenticationMethodEntity>
{
    public void Configure(EntityTypeBuilder<AuthenticationMethodEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasEnumConversion();
        builder.Property(x => x.Method).HasEnumConversion();
        builder.UseTpcMappingStrategy();
    }
}