using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public class ClientTokenAuthMethodConfiguration : IEntityTypeConfiguration<ClientTokenAuthMethodEntity>
{
    public void Configure(EntityTypeBuilder<ClientTokenAuthMethodEntity> builder)
    {
        builder.HasKey(e => new { e.ClientId, e.MethodId });
        
        builder.HasOne(e => e.Client)
            .WithMany(c => c.TokenAuthMethods)
            .HasForeignKey(e => e.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(e => e.Method)
            .WithMany()
            .HasForeignKey(e => e.MethodId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}