using eSecurity.Server.Data.Entities;
using eSystem.Core.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class UserPhoneNumberConfiguration : IEntityTypeConfiguration<UserPhoneNumberEntity>
{
    public void Configure(EntityTypeBuilder<UserPhoneNumberEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PhoneNumber).HasMaxLength(18);
        builder.Property(x => x.Type).HasEnumConversion();
        builder.HasOne(u => u.User)
            .WithMany(x => x.PhoneNumbers)
            .HasForeignKey(x => x.UserId);
    }
}