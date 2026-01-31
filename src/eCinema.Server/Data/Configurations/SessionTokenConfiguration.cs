using eCinema.Server.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCinema.Server.Data.Configurations;

public class SessionTokenConfiguration : IEntityTypeConfiguration<SessionTokenEntity>
{
    public void Configure(EntityTypeBuilder<SessionTokenEntity> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(x => x.TokenType).HasMaxLength(100);
        builder.Property(x => x.EncryptedValue).HasMaxLength(3000);
    }
}