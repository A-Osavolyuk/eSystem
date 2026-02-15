using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class PublicSubjectConfiguration : IEntityTypeConfiguration<PublicSubjectEntity>
{
    public void Configure(EntityTypeBuilder<PublicSubjectEntity> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(x => x.Subject).HasMaxLength(36);
        
        builder.HasOne(x => x.User)
            .WithOne(x => x.PublicSubject)
            .HasForeignKey<PublicSubjectEntity>(x => x.UserId);
    }
}