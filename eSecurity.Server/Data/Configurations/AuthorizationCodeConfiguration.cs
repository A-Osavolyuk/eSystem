using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class AuthorizationCodeConfiguration : IEntityTypeConfiguration<AuthorizationCodeEntity>
{
    public void Configure(EntityTypeBuilder<AuthorizationCodeEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RedirectUri).HasMaxLength(200);
        builder.Property(x => x.CodeChallenge).HasMaxLength(200);
        builder.Property(x => x.CodeChallengeMethod).HasMaxLength(16);
        builder.Property(x => x.Code).HasMaxLength(20);
        builder.Property(x => x.Nonce).HasMaxLength(32);

        builder.HasOne(x => x.Device)
            .WithMany()
            .HasForeignKey(x => x.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);;

        builder.HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);;
    }
}