using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class ResponseTypeConfiguration : IEntityTypeConfiguration<ResponseTypeEntity>
{
    public void Configure(EntityTypeBuilder<ResponseTypeEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasMaxLength(60);
    }
}