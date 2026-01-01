using eSecurity.Server.Data.Entities;
using eSystem.Core.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class OAuthSessionConfiguration : IEntityTypeConfiguration<OAuthSessionEntity>
{
    public void Configure(EntityTypeBuilder<OAuthSessionEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Token).HasMaxLength(32);
        builder.Property(x => x.Flow).HasEnumConversion();

        builder.HasOne(x => x.LinkedAccount)
            .WithMany()
            .HasForeignKey(x => x.LinkedAccountId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}