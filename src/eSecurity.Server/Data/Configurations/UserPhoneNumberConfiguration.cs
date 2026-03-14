using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data.Conversion;
using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class UserPhoneNumberConfiguration : IEntityTypeConfiguration<UserPhoneNumberEntity>
{
    public void Configure(EntityTypeBuilder<UserPhoneNumberEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PhoneNumber).HasMaxLength(18);
        builder.Property(x => x.Type).HasConversion<EnumValueConverter<PhoneNumberType>>();
        
        builder.HasOne(u => u.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}