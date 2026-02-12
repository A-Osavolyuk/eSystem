using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class TokenAuthMethodConfiguration : IEntityTypeConfiguration<TokenAuthMethodEntity>
{
    public void Configure(EntityTypeBuilder<TokenAuthMethodEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Method).HasMaxLength(60);
    }
}