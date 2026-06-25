using eSecurity.Idp.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Idp.Data.Configurations;

public sealed class SoftwateKeyConfiguration : IEntityTypeConfiguration<SoftwareKeyEntity>
{
    public void Configure(EntityTypeBuilder<SoftwareKeyEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CredentialId).HasMaxLength(1000);
        builder.Property(x => x.Domain).HasMaxLength(100);
        builder.Property(x => x.DisplayName).HasMaxLength(100);
        builder.Property(x => x.Type).HasMaxLength(32);

        builder.HasOne(x => x.Device)
            .WithOne()
            .HasForeignKey<SoftwareKeyEntity>(x => x.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}