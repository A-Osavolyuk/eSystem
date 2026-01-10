using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Username).HasMaxLength(64);
        builder.Property(x => x.NormalizedUsername).HasMaxLength(64);
        builder.Property(x => x.ZoneInfo).HasMaxLength(32);
        builder.Property(x => x.Locale).HasMaxLength(10);
    }
}