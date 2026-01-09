using eSecurity.Server.Data.Entities;
using eSystem.Core.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class CodeConfiguration : IEntityTypeConfiguration<CodeEntity>
{
    public void Configure(EntityTypeBuilder<CodeEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CodeHash).HasMaxLength(200);
        builder.Property(x => x.Action).HasConversion<string>();
        builder.Property(x => x.Sender).HasConversion<string>();
        builder.Property(x => x.Purpose).HasConversion<string>();
    }
}
