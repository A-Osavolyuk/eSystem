using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class PairwiseSubjectConfiguration : IEntityTypeConfiguration<PairwiseSubjectEntity>
{
    public void Configure(EntityTypeBuilder<PairwiseSubjectEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SectorIdentifier).HasMaxLength(100);
        builder.Property(x => x.Subject).HasMaxLength(256);
            
        builder.HasOne(x => x.Client)
            .WithMany(x => x.PairwiseSubjects)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}