using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class ClientBackChannelLogoutUriConfiguration : IEntityTypeConfiguration<ClientBackChannelLogoutUriEntity>
{
    public void Configure(EntityTypeBuilder<ClientBackChannelLogoutUriEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Uri).HasMaxLength(200);

        builder.HasOne(x => x.Client)
            .WithMany(x => x.BackChannelLogoutUris)
            .HasForeignKey(x => x.ClientId);
    }
}