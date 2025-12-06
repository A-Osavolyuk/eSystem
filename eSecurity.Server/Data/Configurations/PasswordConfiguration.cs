using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class PasswordConfiguration : IEntityTypeConfiguration<PasswordEntity>
{
    public void Configure(EntityTypeBuilder<PasswordEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Hash).HasMaxLength(1000);
        builder.HasOne(x => x.User)
            .WithOne(x => x.Password)
            .HasForeignKey<PasswordEntity>(x => x.UserId);
    }
}