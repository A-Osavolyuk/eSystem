using eSecurity.Idp.Data.Entities;
using eSystem.Core.Server.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Idp.Data.Configurations;

public sealed class EndSessionRequestConfiguration : IEntityTypeConfiguration<EndSessionRequestEntity>
{
    public void Configure(EntityTypeBuilder<EndSessionRequestEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.IdTokenHint).HasMaxLength(1000);
        builder.Property(x => x.State).HasMaxLength(1000);
        builder.Property(x => x.PostLogoutRedirectUri).HasMaxLength(1000);
        builder.Property(x => x.LogoutHint).HasMaxLength(32);
        builder.Property(x => x.Status).HasEnumConversion();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Session)
            .WithMany()
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}