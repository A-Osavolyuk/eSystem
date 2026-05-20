using eSecurity.Idp.Data.Entities;
using eSystem.Core.Server.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Idp.Data.Configurations;

public sealed class DeviceCodeConfiguration : IEntityTypeConfiguration<DeviceCodeEntity>
{
    public void Configure(EntityTypeBuilder<DeviceCodeEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Hash).HasMaxLength(200);
        builder.Property(x => x.UserCode).HasMaxLength(9);
        builder.Property(x => x.DeviceName).HasMaxLength(50);
        builder.Property(x => x.DeviceModel).HasMaxLength(100);
        builder.Property(x => x.DenyReason).HasMaxLength(100);
        builder.Property(x => x.State).HasEnumConversion();

        builder.HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Session)
            .WithMany()
            .HasForeignKey(x => x.SessionId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}