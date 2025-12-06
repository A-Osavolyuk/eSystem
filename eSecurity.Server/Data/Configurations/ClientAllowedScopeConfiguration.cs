using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class ClientAllowedScopeConfiguration : IEntityTypeConfiguration<ClientAllowedScopeEntity>
{
    public void Configure(EntityTypeBuilder<ClientAllowedScopeEntity> builder)
    {
        builder.HasKey(x => new { x.ClientId, x.ScopeId });

        builder.HasOne(x => x.Client)
            .WithMany(x => x.AllowedScopes)
            .HasForeignKey(x => x.ClientId);

        builder.HasOne(x => x.Scope)
            .WithMany()
            .HasForeignKey(x => x.ScopeId);
    }
}