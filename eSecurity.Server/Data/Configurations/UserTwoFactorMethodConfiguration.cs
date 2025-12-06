using eSecurity.Server.Data.Entities;
using eSystem.Core.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class UserTwoFactorMethodConfiguration : IEntityTypeConfiguration<UserTwoFactorMethodEntity>
{
    public void Configure(EntityTypeBuilder<UserTwoFactorMethodEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Method).HasEnumConversion();
        builder.HasOne(x => x.User)
            .WithMany(x => x.TwoFactorMethods)
            .HasForeignKey(x => x.UserId);
    }
}