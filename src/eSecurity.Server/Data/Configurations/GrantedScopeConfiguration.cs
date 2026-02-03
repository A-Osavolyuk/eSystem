using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class GrantedScopeConfiguration : IEntityTypeConfiguration<GrantedScopeEntity>
{
    public void Configure(EntityTypeBuilder<GrantedScopeEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Consent)
            .WithMany(x => x.GrantedScopes)
            .HasForeignKey(x => x.ConsentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}