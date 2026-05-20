using eSecurity.Idp.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Idp.Data.Configurations;

public sealed class SigningCertificateConfiguration : IEntityTypeConfiguration<SigningCertificateEntity>
{
    public void Configure(EntityTypeBuilder<SigningCertificateEntity> builder)
    {
        builder.HasKey(x => x.Id);
    }
}