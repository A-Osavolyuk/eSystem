using eSecurity.Server.Data.Entities;
using eSystem.Core.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class UserLinkedAccountConfiguration : IEntityTypeConfiguration<UserLinkedAccountEntity>
{
    public void Configure(EntityTypeBuilder<UserLinkedAccountEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasEnumConversion();

        builder.HasOne(x => x.User)
            .WithMany(x => x.LinkedAccounts)
            .HasForeignKey(x => x.UserId);
    }
}