using System.Text.Json;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.SignIn.Session;
using eSystem.Core.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eSecurity.Server.Data.Configurations;

public class SignInSessionConfiguration : IEntityTypeConfiguration<SignInSessionEntity>
{
    public void Configure(EntityTypeBuilder<SignInSessionEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasEnumConversion();
        builder.Property(x => x.CurrentStep).HasEnumConversion();

        builder.Property(x => x.RequiredSteps)
            .HasConversion(
                v => JsonSerializer.Serialize(v),
                v => JsonSerializer.Deserialize<List<SignInStep>>(v) ?? new List<SignInStep>()
            )
            .Metadata.SetValueComparer(
                new ValueComparer<IReadOnlyCollection<SignInStep>>(
                    (c1, c2) 
                        => JsonSerializer.Serialize(c1) == JsonSerializer.Serialize(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => new List<SignInStep>(c)
                )
            );

        builder.Property(x => x.CompletedSteps)
            .HasConversion(
                v => JsonSerializer.Serialize(v),
                v => JsonSerializer.Deserialize<HashSet<SignInStep>>(v) ?? new HashSet<SignInStep>()
            )
            .Metadata.SetValueComparer(
                new ValueComparer<HashSet<SignInStep>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SetEquals(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => new HashSet<SignInStep>(c)
                )
            );

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
    }
}