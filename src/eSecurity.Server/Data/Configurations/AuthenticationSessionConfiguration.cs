using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class AuthenticationSessionConfiguration : IEntityTypeConfiguration<AuthenticationSessionEntity>
{
    public void Configure(EntityTypeBuilder<AuthenticationSessionEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.IdentityProvider).HasMaxLength(32);
        builder.Property(x => x.OAuthFlow).HasConversion<string>();
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Session)
            .WithMany()
            .HasForeignKey(x => x.SessionId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}