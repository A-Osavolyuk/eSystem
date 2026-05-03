using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class LogoutRequestConfiguration : IEntityTypeConfiguration<EndSessionRequestEntity>
{
    public void Configure(EntityTypeBuilder<EndSessionRequestEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.IdTokenHint).HasMaxLength(1000);
        builder.Property(x => x.PostLogoutRedirectUri).HasMaxLength(256);
        builder.Property(x => x.State).HasMaxLength(1000);
        builder.Property(x => x.ClientId).HasMaxLength(36);
        builder.Property(x => x.LogoutHint).HasMaxLength(64);
        builder.Property(x => x.Status).HasEnumConversion();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}