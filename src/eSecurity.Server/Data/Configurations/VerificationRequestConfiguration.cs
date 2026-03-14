using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Server.Data.Conversion;
using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class VerificationRequestConfiguration : IEntityTypeConfiguration<VerificationRequestEntity>
{
    public void Configure(EntityTypeBuilder<VerificationRequestEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Action).HasConversion<EnumValueConverter<ActionType>>();
        builder.Property(x => x.Purpose).HasConversion<EnumValueConverter<PurposeType>>();
        builder.Property(x => x.Status).HasConversion<EnumValueConverter<VerificationStatus>>();
        builder.Property(x => x.Method).HasConversion<EnumValueConverter<VerificationMethod>>();
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}