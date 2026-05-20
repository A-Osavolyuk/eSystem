using eSecurity.Idp.Data.Entities;
using eSystem.Core.Server.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Idp.Data.Configurations;

public sealed class UserPhoneNumberConfiguration : IEntityTypeConfiguration<UserPhoneNumberEntity>
{
    public void Configure(EntityTypeBuilder<UserPhoneNumberEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PhoneNumber).HasMaxLength(18);
        builder.Property(x => x.Type).HasEnumConversion();
        
        builder.HasOne(u => u.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}