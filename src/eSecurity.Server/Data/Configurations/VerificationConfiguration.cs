using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class VerificationConfiguration : IEntityTypeConfiguration<VerificationEntity>
{
    public void Configure(EntityTypeBuilder<VerificationEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Purpose).HasConversion<string>();
        builder.Property(x => x.Action).HasConversion<string>();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}