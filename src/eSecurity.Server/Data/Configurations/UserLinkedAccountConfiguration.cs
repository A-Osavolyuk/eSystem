using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Server.Data.Conversion;
using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class UserLinkedAccountConfiguration : IEntityTypeConfiguration<UserLinkedAccountEntity>
{
    public void Configure(EntityTypeBuilder<UserLinkedAccountEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasConversion<EnumValueConverter<LinkedAccountType>>();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}