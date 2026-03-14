using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Server.Data.Conversion;
using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class VerificationConfiguration : IEntityTypeConfiguration<VerificationEntity>
{
    public void Configure(EntityTypeBuilder<VerificationEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Purpose).HasConversion<EnumValueConverter<PurposeType>>();
        builder.Property(x => x.Action).HasConversion<EnumValueConverter<ActionType>>();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}