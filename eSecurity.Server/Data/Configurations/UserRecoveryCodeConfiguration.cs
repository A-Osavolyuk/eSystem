using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class UserRecoveryCodeConfiguration : IEntityTypeConfiguration<UserRecoveryCodeEntity>
{
    public void Configure(EntityTypeBuilder<UserRecoveryCodeEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProtectedCode).HasMaxLength(150);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
    }
}