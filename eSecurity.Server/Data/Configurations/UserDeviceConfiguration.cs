using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class UserDeviceConfiguration : IEntityTypeConfiguration<UserDeviceEntity>
{
    public void Configure(EntityTypeBuilder<UserDeviceEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Device).HasMaxLength(64);
        builder.Property(x => x.Browser).HasMaxLength(64);
        builder.Property(x => x.Os).HasMaxLength(64);
        builder.Property(x => x.UserAgent).HasMaxLength(128);
        builder.Property(x => x.Location).HasMaxLength(128);
        builder.Property(x => x.IpAddress).HasMaxLength(15);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
    }
}