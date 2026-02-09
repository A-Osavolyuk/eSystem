using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class OAuthSessionConfiguration : IEntityTypeConfiguration<OAuthSessionEntity>
{
    public void Configure(EntityTypeBuilder<OAuthSessionEntity> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(x => x.Provider).HasMaxLength(32);
        builder.Property(x => x.Flow).HasConversion<string>();
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}