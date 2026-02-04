using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class DeviceCodeConfiguration : IEntityTypeConfiguration<DeviceCodeEntity>
{
    public void Configure(EntityTypeBuilder<DeviceCodeEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DeviceCodeHash).HasMaxLength(200);
        builder.Property(x => x.UserCode).HasMaxLength(9);
        builder.Property(x => x.Scope).HasMaxLength(200);
        builder.Property(x => x.State).HasConversion<string>();

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