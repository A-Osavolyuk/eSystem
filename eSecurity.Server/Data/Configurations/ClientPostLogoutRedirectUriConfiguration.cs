using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class ClientPostLogoutRedirectUriConfiguration : IEntityTypeConfiguration<ClientPostLogoutRedirectUriEntity>
{
    public void Configure(EntityTypeBuilder<ClientPostLogoutRedirectUriEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Uri).HasMaxLength(200);

        builder.HasOne(x => x.Client)
            .WithMany(x => x.PostLogoutRedirectUris)
            .HasForeignKey(x => x.ClientId);
    }
}