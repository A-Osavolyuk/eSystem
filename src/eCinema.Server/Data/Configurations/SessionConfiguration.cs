using eCinema.Server.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCinema.Server.Data.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<SessionEntity>
{
    public void Configure(EntityTypeBuilder<SessionEntity> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.HasIndex(x => x.SessionKey).IsUnique();
        builder.HasIndex(x => x.Sid);
        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.SessionKey).HasMaxLength(100);
        builder.Property(x => x.UserId).HasMaxLength(36);
        builder.Property(x => x.Sid).HasMaxLength(36);
        
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