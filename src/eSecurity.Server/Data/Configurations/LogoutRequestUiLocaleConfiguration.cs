using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class LogoutRequestUiLocaleConfiguration : IEntityTypeConfiguration<LogoutRequestUiLocaleEntity>
{
    public void Configure(EntityTypeBuilder<LogoutRequestUiLocaleEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Locale).HasMaxLength(32);

        builder.HasOne(x => x.Request)
            .WithMany(x => x.UiLocales)
            .HasForeignKey(x => x.RequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}