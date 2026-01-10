using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class UserLockoutStateConfiguration : IEntityTypeConfiguration<UserLockoutStateEntity>
{
    public void Configure(EntityTypeBuilder<UserLockoutStateEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Description).HasMaxLength(3000);

        builder.HasOne(p => p.User)
            .WithOne()
            .HasForeignKey<UserLockoutStateEntity>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}