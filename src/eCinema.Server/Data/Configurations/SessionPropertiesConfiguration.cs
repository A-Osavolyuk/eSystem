using eCinema.Server.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCinema.Server.Data.Configurations;

public class SessionPropertiesConfiguration : IEntityTypeConfiguration<SessionPropertiesEntity>
{
    public void Configure(EntityTypeBuilder<SessionPropertiesEntity> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(x => x.Acr).HasMaxLength(1000);
        builder.Property(x => x.Amr).HasMaxLength(1000);
        builder.Property(x => x.RedirectUri).HasMaxLength(1000);
    }
}