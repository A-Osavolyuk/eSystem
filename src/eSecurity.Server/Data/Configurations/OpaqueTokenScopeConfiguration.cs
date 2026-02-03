using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class OpaqueTokenScopeConfiguration : IEntityTypeConfiguration<OpaqueTokenScopeEntity>
{
    public void Configure(EntityTypeBuilder<OpaqueTokenScopeEntity> builder)
    {
        builder.HasKey(x => x.Id);
            
        builder.HasOne(x => x.Token)
            .WithMany(x => x.Scopes)
            .HasForeignKey(x => x.TokenId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}