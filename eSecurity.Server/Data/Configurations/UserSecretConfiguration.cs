using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class UserSecretConfiguration : IEntityTypeConfiguration<UserSecretEntity>
{
    public void Configure(EntityTypeBuilder<UserSecretEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Secret).HasMaxLength(200);
        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<UserSecretEntity>(x => x.UserId);
    }
}