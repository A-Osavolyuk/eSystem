using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class ClientFrontChannelLogoutUriConfiguration : IEntityTypeConfiguration<ClientFrontChannelLogoutUriEntity>
{
    public void Configure(EntityTypeBuilder<ClientFrontChannelLogoutUriEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Uri).HasMaxLength(200);

        builder.HasOne(x => x.Client)
            .WithMany(x => x.FrontChannelLogoutUris)
            .HasForeignKey(x => x.ClientId);
    }
}