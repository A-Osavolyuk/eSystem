using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public class ClientSessionConfiguration : IEntityTypeConfiguration<ClientSessionEntity>
{
    public void Configure(EntityTypeBuilder<ClientSessionEntity> builder)
    {
        builder.HasKey(x => new { x.ClientId, x.SessionId });
        
        builder.HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(f => f.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Session)
            .WithMany()
            .HasForeignKey(f => f.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}