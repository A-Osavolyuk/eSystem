using eSecurity.Idp.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Idp.Data.Configurations;

public sealed class PushedAuthorizationRequestScopeConfiguration 
    : IEntityTypeConfiguration<PushedAuthorizationRequestScopeEntity>
{
    public void Configure(EntityTypeBuilder<PushedAuthorizationRequestScopeEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Scope).HasMaxLength(100);
    }
}