using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class ClientAllowedScopeConfiguration : IEntityTypeConfiguration<ClientAllowedScopeEntity>
{
    public void Configure(EntityTypeBuilder<ClientAllowedScopeEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Client)
            .WithMany(x => x.AllowedScopes)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}