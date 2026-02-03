using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class ClientResponseTypeConfiguration : IEntityTypeConfiguration<ClientResponseTypeEntity>
{
    public void Configure(EntityTypeBuilder<ClientResponseTypeEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ResponseType).HasMaxLength(32);
        
        builder.HasOne(x => x.Client)
            .WithMany(x => x.ResponseTypes)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}