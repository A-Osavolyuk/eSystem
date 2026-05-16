using eSecurity.Client.BFF.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Client.BFF.Data.Configurations;

public sealed class SessionClaimConfiguration : IEntityTypeConfiguration<SessionClaimEntity>
{
    public void Configure(EntityTypeBuilder<SessionClaimEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Value)
            .HasMaxLength(200)
            .IsRequired();
    }
}