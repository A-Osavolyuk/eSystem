using eSecurity.Idp.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Idp.Data.Configurations;

public sealed class ClientAllowedScopeConfiguration : IEntityTypeConfiguration<ClientAllowedScopeEntity>
{
    public void Configure(EntityTypeBuilder<ClientAllowedScopeEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Scope)
            .WithMany()
            .HasForeignKey(x => x.ScopeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}