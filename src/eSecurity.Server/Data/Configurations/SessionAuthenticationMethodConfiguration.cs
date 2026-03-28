using eSecurity.Server.Data.Entities;
using eSystem.Core.Data.Conversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class SessionAuthenticationMethodConfiguration 
    : IEntityTypeConfiguration<SessionAuthenticationMethodEntity>
{
    public void Configure(EntityTypeBuilder<SessionAuthenticationMethodEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Method).HasEnumConversion();
        
        builder.HasOne(x => x.Session)
            .WithMany(x => x.AuthenticationMethods)
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}