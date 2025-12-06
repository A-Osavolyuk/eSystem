using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class PasskeyConfiguration : IEntityTypeConfiguration<PasskeyEntity>
{
    public void Configure(EntityTypeBuilder<PasskeyEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CredentialId).HasMaxLength(1000);
        builder.Property(x => x.Domain).HasMaxLength(100);
        builder.Property(x => x.DisplayName).HasMaxLength(100);
        builder.Property(x => x.Type).HasMaxLength(32);

        builder.HasOne(x => x.Device)
            .WithOne(x => x.Passkey)
            .HasForeignKey<PasskeyEntity>(x => x.DeviceId);
    }
}