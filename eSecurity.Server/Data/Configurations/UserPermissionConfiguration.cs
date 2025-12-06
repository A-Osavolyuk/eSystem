using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermissionsEntity>
{
    public void Configure(EntityTypeBuilder<UserPermissionsEntity> builder)
    {
        builder.HasKey(ur => new { ur.UserId, Id = ur.PermissionId });

        builder.HasOne(ur => ur.User)
            .WithMany(u => u.Permissions)
            .HasForeignKey(ur => ur.UserId);

        builder.HasOne(ur => ur.Permission)
            .WithMany()
            .HasForeignKey(ur => ur.PermissionId);
    }
}