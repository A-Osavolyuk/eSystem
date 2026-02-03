using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public class ClientTokenAuthMethodConfiguration : IEntityTypeConfiguration<ClientTokenAuthMethodEntity>
{
    public void Configure(EntityTypeBuilder<ClientTokenAuthMethodEntity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Method).HasMaxLength(32);
        
        builder.HasOne(e => e.Client)
            .WithMany(c => c.TokenAuthMethods)
            .HasForeignKey(e => e.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}