using eSecurity.Idp.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Idp.Data.Configurations;

public sealed class UserTwoFactorMethodConfiguration : IEntityTypeConfiguration<UserTwoFactorMethodEntity>
{
    public void Configure(EntityTypeBuilder<UserTwoFactorMethodEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Method)
            .WithMany()
            .HasForeignKey(x => x.MethodId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}