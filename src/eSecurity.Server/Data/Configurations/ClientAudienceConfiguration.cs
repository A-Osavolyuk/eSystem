using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class ClientAudienceConfiguration : IEntityTypeConfiguration<ClientAudienceEntity>
{
    public void Configure(EntityTypeBuilder<ClientAudienceEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Audience).HasMaxLength(200);
        
        builder.HasOne(x => x.Client)
            .WithMany(x => x.Audiences)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}