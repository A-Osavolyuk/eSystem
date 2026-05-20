using eSecurity.Idp.Data.Entities;
using eSystem.Core.Server.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Idp.Data.Configurations;

public sealed class UserTwoFactorMethodConfiguration : IEntityTypeConfiguration<UserTwoFactorMethodEntity>
{
    public void Configure(EntityTypeBuilder<UserTwoFactorMethodEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Method).HasEnumConversion();
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}