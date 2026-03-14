using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class UserTwoFactorMethodConfiguration : IEntityTypeConfiguration<UserTwoFactorMethodEntity>
{
    public void Configure(EntityTypeBuilder<UserTwoFactorMethodEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Method).HasConversion<EnumValueConverter<TwoFactorMethod>>();
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}