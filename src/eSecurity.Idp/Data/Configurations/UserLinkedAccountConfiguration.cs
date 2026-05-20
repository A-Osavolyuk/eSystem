using eSecurity.Idp.Data.Entities;
using eSystem.Core.Server.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Idp.Data.Configurations;

public sealed class UserLinkedAccountConfiguration : IEntityTypeConfiguration<UserLinkedAccountEntity>
{
    public void Configure(EntityTypeBuilder<UserLinkedAccountEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasEnumConversion();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}