using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class AuthenticationSessionConfiguration : IEntityTypeConfiguration<AuthenticationSessionEntity>
{
    public void Configure(EntityTypeBuilder<AuthenticationSessionEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.IdentityProvider).HasMaxLength(32);
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}