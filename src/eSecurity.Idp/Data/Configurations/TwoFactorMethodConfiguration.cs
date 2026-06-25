using eSecurity.Idp.Data.Entities;
using eSystem.Core.Server.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Idp.Data.Configurations;

public sealed class TwoFactorMethodConfiguration : IEntityTypeConfiguration<TwoFactorMethodEntity>
{
    public void Configure(EntityTypeBuilder<TwoFactorMethodEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasEnumConversion();
        builder.Property(x => x.Name).HasMaxLength(30);
        builder.Property(x => x.Description).HasMaxLength(500);
    }
}