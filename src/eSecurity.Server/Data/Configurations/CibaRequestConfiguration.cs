using eSecurity.Server.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class CibaRequestConfiguration : IEntityTypeConfiguration<CibaRequestEntity>
{
    public void Configure(EntityTypeBuilder<CibaRequestEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.State).HasConversion<string>();
        builder.Property(x => x.AuthReqId).HasMaxLength(36);
        builder.Property(x => x.UserCode).HasMaxLength(8);
        builder.Property(x => x.AcrValues).HasMaxLength(100);
        builder.Property(x => x.Scope).HasMaxLength(100);
        builder.Property(x => x.BindingMessage).HasMaxLength(500);
        builder.Property(x => x.DeniedReason).HasMaxLength(500);
        
        builder.HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(y => y.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(y => y.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Session)
            .WithMany()
            .HasForeignKey(y => y.SessionId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }
}