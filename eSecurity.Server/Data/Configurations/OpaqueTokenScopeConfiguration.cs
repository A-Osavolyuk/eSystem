using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class OpaqueTokenScopeConfiguration : IEntityTypeConfiguration<OpaqueTokenScopeEntity>
{
    public void Configure(EntityTypeBuilder<OpaqueTokenScopeEntity> builder)
    {
        builder.HasKey(x => new { x.TokenId, x.ScopeId });
            
        builder.HasOne(x => x.Token)
            .WithMany(x => x.Scopes)
            .HasForeignKey(x => x.TokenId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(x => x.Scope)
            .WithMany()
            .HasForeignKey(x => x.ScopeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}