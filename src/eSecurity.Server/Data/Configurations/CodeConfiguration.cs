using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authorization.Codes;
using eSystem.Core.Common.Messaging;
using eSystem.Core.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class CodeConfiguration : IEntityTypeConfiguration<CodeEntity>
{
    public void Configure(EntityTypeBuilder<CodeEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CodeHash).HasMaxLength(200);
        builder.Property(x => x.Sender).HasEnumConversion();
        builder.Property(x => x.State).HasEnumConversion();
    }
}
