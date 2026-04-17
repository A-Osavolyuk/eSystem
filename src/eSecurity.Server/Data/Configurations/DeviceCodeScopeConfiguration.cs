using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class DeviceCodeScopeConfiguration : IEntityTypeConfiguration<DeviceCodeScopeEntity>
{
    public void Configure(EntityTypeBuilder<DeviceCodeScopeEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Scope).HasMaxLength(32);
        builder.HasOne(x => x.DeviceCode)
            .WithMany(x => x.Scopes)
            .HasForeignKey(x => x.DeviceCodeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}