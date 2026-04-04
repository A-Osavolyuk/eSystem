using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class GrantTypeConfiguration : IEntityTypeConfiguration<GrantTypeEntity>
{
    public void Configure(EntityTypeBuilder<GrantTypeEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Grant).HasEnumConversion();
    }
}