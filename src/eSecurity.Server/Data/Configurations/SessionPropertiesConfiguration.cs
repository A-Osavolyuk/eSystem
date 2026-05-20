using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class SessionPropertiesConfiguration : IEntityTypeConfiguration<SessionPropertiesEntity>
{
    public void Configure(EntityTypeBuilder<SessionPropertiesEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RedirectUri).HasMaxLength(1000);
    }
}