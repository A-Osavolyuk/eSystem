using System.Collections.Immutable;
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

        builder.Property(x => x.RequiredSteps).HasConversion(
                v => string.Join(',', v.Select(x => x.ToString())),
                v => v.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(Enum.Parse<SignInStep>)
                    .ToList())
            .Metadata.SetValueComparer(new ValueComparer<IReadOnlyCollection<SignInStep>>(
                (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => new List<SignInStep>(c)));

        builder.Property(x => x.CompletedSteps)
            .HasConversion(
                v => string.Join(',', v.Select(x => x.ToString())),
                v => v.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(Enum.Parse<SignInStep>)
                    .ToHashSet())
            .Metadata
            .SetValueComparer(
                new ValueComparer<HashSet<SignInStep>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SetEquals(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => new HashSet<SignInStep>(c)
                ));

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
    }
}