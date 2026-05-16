using eSecurity.Client.BFF.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Client.BFF.Data.Configurations;

public sealed class SessionTokenConfiguration : IEntityTypeConfiguration<SessionTokenEntity>
{
    public void Configure(EntityTypeBuilder<SessionTokenEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TokenType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.EncryptedValue)
            .HasMaxLength(2000)
            .IsRequired();
    }
}