using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class ClientGrantTypeConfiguration : IEntityTypeConfiguration<ClientGrantTypeEntity>
{
    public void Configure(EntityTypeBuilder<ClientGrantTypeEntity> builder)
    {
       builder.HasKey(x => x.Id);
       builder.Property(x => x.Type).HasMaxLength(50);

        builder.HasOne(x => x.Client)
            .WithMany(x => x.GrantTypes)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}