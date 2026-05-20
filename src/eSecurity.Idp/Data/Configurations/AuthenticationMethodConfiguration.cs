using eSecurity.Idp.Data.Entities;
using eSystem.Core.Server.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Idp.Data.Configurations;

public sealed class AuthenticationMethodConfiguration : IEntityTypeConfiguration<AuthenticationMethodEntity>
{
    public void Configure(EntityTypeBuilder<AuthenticationMethodEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasEnumConversion();
        builder.Property(x => x.MethodReference).HasEnumConversion();
    }
}