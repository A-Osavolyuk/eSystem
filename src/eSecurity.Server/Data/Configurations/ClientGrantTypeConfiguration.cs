using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class ClientGrantTypeConfiguration : IEntityTypeConfiguration<ClientGrantTypeEntity>
{
    public void Configure(EntityTypeBuilder<ClientGrantTypeEntity> builder)
    {
       builder.HasKey(x => new { x.ClientId, x.GrantId });

        builder.HasOne(x => x.Client)
            .WithMany(x => x.GrantTypes)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Grant)
            .WithMany()
            .HasForeignKey(x => x.GrantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}