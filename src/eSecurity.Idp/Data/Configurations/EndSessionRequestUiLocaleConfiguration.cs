using eSecurity.Idp.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Idp.Data.Configurations;

public sealed class EndSessionRequestUiLocaleConfiguration : IEntityTypeConfiguration<EndSessionRequestUiLocaleEntity>
{
    public void Configure(EntityTypeBuilder<EndSessionRequestUiLocaleEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Locale).HasMaxLength(32);

        builder.HasOne(x => x.Request)
            .WithMany(x => x.UiLocales)
            .HasForeignKey(x => x.RequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}