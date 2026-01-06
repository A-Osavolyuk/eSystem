using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public class LoginSessionConfiguration : IEntityTypeConfiguration<LoginSessionEntity>
{
    public void Configure(EntityTypeBuilder<LoginSessionEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
    }
}