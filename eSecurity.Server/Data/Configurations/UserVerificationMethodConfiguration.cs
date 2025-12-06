using eSecurity.Server.Data.Entities;
using eSystem.Core.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class UserVerificationMethodConfiguration : IEntityTypeConfiguration<UserVerificationMethodEntity>
{
    public void Configure(EntityTypeBuilder<UserVerificationMethodEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(x => x.Method).HasEnumConversion();

        builder.HasOne(x => x.User)
            .WithMany(x => x.VerificationMethods)
            .HasForeignKey(x => x.UserId);
    }
}