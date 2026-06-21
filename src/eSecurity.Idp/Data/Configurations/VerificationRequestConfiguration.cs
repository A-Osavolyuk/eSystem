using eSecurity.Idp.Data.Entities;
using eSystem.Core.Server.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Idp.Data.Configurations;

public sealed class VerificationRequestConfiguration : IEntityTypeConfiguration<VerificationRequestEntity>
{
    public void Configure(EntityTypeBuilder<VerificationRequestEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Status).HasEnumConversion();
        builder.Property(x => x.Method).HasEnumConversion();
        builder.Property(x => x.Operation).HasEnumConversion();

        builder.Property(x => x.Payload).HasColumnType("jsonb");
        builder.Property(x => x.Target).HasMaxLength(50);
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}