using eSecurity.Server.Data.Entities;
using eSystem.Core.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class OpaqueTokenConfiguration : IEntityTypeConfiguration<OpaqueTokenEntity>
{
    public void Configure(EntityTypeBuilder<OpaqueTokenEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TokenHash).HasMaxLength(100);
        builder.Property(x => x.TokenType).HasEnumConversion();
            
        builder.HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(x => x.Session)
            .WithMany(x => x.OpaqueTokens)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}