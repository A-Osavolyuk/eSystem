using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class ResourceOwnerConfiguration : IEntityTypeConfiguration<ResourceOwnerEntity>
{
    public void Configure(EntityTypeBuilder<ResourceOwnerEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(64);
    }
}