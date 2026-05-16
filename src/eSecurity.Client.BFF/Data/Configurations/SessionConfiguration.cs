using eSecurity.Client.BFF.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Client.BFF.Data.Configurations;

public sealed class SessionConfiguration : IEntityTypeConfiguration<SessionEntity>
{
    public void Configure(EntityTypeBuilder<SessionEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Key)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(x => x.Sid)
            .HasMaxLength(36)
            .IsRequired();

        builder.HasOne(x => x.Properties)
            .WithOne(x => x.Session)
            .HasForeignKey<SessionPropertiesEntity>(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Claims)
            .WithOne(x => x.Session)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Tokens)
            .WithOne(x => x.Session)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}