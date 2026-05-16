using eSecurity.Server.Data.Entities;
using eSystem.Core.Server.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class PushedAuthorizationRequestPromptConfiguration 
    : IEntityTypeConfiguration<PushedAuthorizationRequestPromptEntity>
{
    public void Configure(EntityTypeBuilder<PushedAuthorizationRequestPromptEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Prompt).HasEnumConversion();
    }
}