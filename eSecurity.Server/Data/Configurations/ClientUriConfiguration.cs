using eSecurity.Server.Data.Entities;
using eSystem.Core.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public class ClientUriConfiguration : IEntityTypeConfiguration<ClientUriEntity>
{
    public void Configure(EntityTypeBuilder<ClientUriEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Uri).HasMaxLength(2048);
        builder.Property(x => x.Type).HasEnumConversion();
        
        builder.HasOne(x => x.Client)
            .WithMany(x => x.Uris)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}