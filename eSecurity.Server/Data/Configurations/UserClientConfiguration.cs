using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class UserClientConfiguration : IEntityTypeConfiguration<UserClientEntity>
{
    public void Configure(EntityTypeBuilder<UserClientEntity> builder)
    {
        builder.HasKey(x => new { x.UserId, x.ClientId });

        builder.HasOne(x => x.User)
            .WithMany(x => x.Clients)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}