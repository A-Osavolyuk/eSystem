using eSecurity.Server.Data.Conversion;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authorization.Codes;
using eSystem.Core.Common.Messaging;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class CodeConfiguration : IEntityTypeConfiguration<CodeEntity>
{
    public void Configure(EntityTypeBuilder<CodeEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CodeHash).HasMaxLength(200);
        builder.Property(x => x.Sender).HasConversion<EnumValueConverter<SenderType>>();
        builder.Property(x => x.State).HasConversion<EnumValueConverter<CodeState>>();
    }
}
