using eSecurity.Idp.Data.Entities;
using eSystem.Core.Server.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Idp.Data.Configurations;

public sealed class UserEmailConfiguration : IEntityTypeConfiguration<UserEmailEntity>
{
    public void Configure(EntityTypeBuilder<UserEmailEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email).HasMaxLength(64);
        builder.Property(x => x.NormalizedEmail).HasMaxLength(64);
        builder.Property(x => x.Type).HasEnumConversion();
        
        builder.HasOne(u => u.User)
            .WithMany(x => x.Emails)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}