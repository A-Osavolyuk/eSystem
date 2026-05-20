using eSecurity.Idp.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Idp.Data.Configurations;

public sealed class PasswordConfiguration : IEntityTypeConfiguration<PasswordEntity>
{
    public void Configure(EntityTypeBuilder<PasswordEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Hash).HasMaxLength(1000);
        
        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<PasswordEntity>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}