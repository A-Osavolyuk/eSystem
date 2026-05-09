using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class PushedAuthorizationRequestConfiguration : IEntityTypeConfiguration<PushedAuthorizationRequestEntity>
{
    public void Configure(EntityTypeBuilder<PushedAuthorizationRequestEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ResponseType).HasEnumConversion();
        builder.Property(x => x.RedirectUri).HasMaxLength(1000);
        builder.Property(x => x.ClientId).HasMaxLength(36);
        builder.Property(x => x.Nonce).HasMaxLength(100);
        builder.Property(x => x.State).HasMaxLength(1000);
        builder.Property(x => x.CodeChallenge).HasMaxLength(100);
        builder.Property(x => x.CodeChallengeMethod).HasEnumConversion();

        builder.HasMany(x => x.Prompts)
            .WithOne(x => x.Request)
            .HasForeignKey(x => x.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Scopes)
            .WithOne(x => x.Request)
            .HasForeignKey(x => x.RequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}