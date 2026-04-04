using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class VerificationRequestConfiguration : IEntityTypeConfiguration<VerificationRequestEntity>
{
    public void Configure(EntityTypeBuilder<VerificationRequestEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Action).HasEnumConversion();
        builder.Property(x => x.Purpose).HasEnumConversion();
        builder.Property(x => x.Status).HasEnumConversion();
        builder.Property(x => x.Method).HasEnumConversion();
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}