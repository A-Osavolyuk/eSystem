using eSecurity.Server.Data.Entities;
using eSystem.Core.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public sealed class PersonalDataConfiguration : IEntityTypeConfiguration<PersonalDataEntity>
{
    public void Configure(EntityTypeBuilder<PersonalDataEntity> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(x => x.Gender).HasEnumConversion();
        builder.Property(x => x.FirstName).HasMaxLength(64);
        builder.Property(x => x.LastName).HasMaxLength(64);
        builder.Property(x => x.MiddleName).HasMaxLength(64);
            
        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<PersonalDataEntity>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne<Address>(e => e.Address, a =>
        {
            a.Property(x => x.StreetAddress).HasMaxLength(128).HasColumnName("StreetAddress");
            a.Property(x => x.Country).HasMaxLength(64).HasColumnName("Country");
            a.Property(x => x.Region).HasMaxLength(64).HasColumnName("Region");
            a.Property(x => x.Locality).HasMaxLength(64).HasColumnName("Locality");
            a.Property(x => x.PostalCode).HasMaxLength(5).HasColumnName("PostalCode");
        });
    }
}