using eSecurity.Idp.Data.Entities;
using eSystem.Core.Server.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Idp.Data.Configurations;

public sealed class TokenAuthMethodConfiguration : IEntityTypeConfiguration<TokenAuthMethodEntity>
{
    public void Configure(EntityTypeBuilder<TokenAuthMethodEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Method).HasEnumConversion();
    }
}