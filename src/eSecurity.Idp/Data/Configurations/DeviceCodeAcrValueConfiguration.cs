using eSecurity.Idp.Data.Entities;
using eSystem.Core.Server.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Idp.Data.Configurations;

public sealed class DeviceCodeAcrValueConfiguration : IEntityTypeConfiguration<DeviceCodeAcrValueEntity>
{
    public void Configure(EntityTypeBuilder<DeviceCodeAcrValueEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Value).HasEnumConversion();
        builder.HasOne(x => x.DeviceCode)
            .WithMany(x => x.AcrValues)
            .HasForeignKey(x => x.DeviceCodeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}