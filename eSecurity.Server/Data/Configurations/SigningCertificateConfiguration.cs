using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class SigningCertificateConfiguration : IEntityTypeConfiguration<SigningCertificateEntity>
{
    public void Configure(EntityTypeBuilder<SigningCertificateEntity> builder)
    {
        builder.HasKey(x => x.Id);
    }
}