using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class ClientRedirectUriConfiguration : IEntityTypeConfiguration<ClientRedirectUriEntity>
{
    public void Configure(EntityTypeBuilder<ClientRedirectUriEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Uri).HasMaxLength(200);

        builder.HasOne(x => x.Client)
            .WithMany(x => x.RedirectUris)
            .HasForeignKey(x => x.ClientId);
    }
}