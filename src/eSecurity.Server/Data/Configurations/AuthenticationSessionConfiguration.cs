using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class AuthenticationSessionConfiguration : IEntityTypeConfiguration<AuthenticationSessionEntity>
{
    public void Configure(EntityTypeBuilder<AuthenticationSessionEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.IdentityProvider).HasMaxLength(32);
        builder.Property(x => x.OAuthFlow).HasEnumConversion();
        
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
        
        builder.HasMany(x => x.RequiredMethods)
            .WithOne(x => x.Session)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.PassedMethods)
            .WithOne(x => x.Session)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.AllowedMfaMethods)
            .WithOne(x => x.Session)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}