using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class OpaqueTokenAudienceConfiguration : IEntityTypeConfiguration<OpaqueTokenAudienceEntity>
{
    public void Configure(EntityTypeBuilder<OpaqueTokenAudienceEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.Token)
            .WithMany(x => x.Audiences)
            .HasForeignKey(x => x.TokenId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Audience)
            .WithMany()
            .HasForeignKey(x => x.AudienceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}