using eSystem.Core.Attributes;

namespace eSystem.Auth.Api.Services;

[Injectable(typeof(IChangeManager), ServiceLifetime.Scoped)]
public sealed class ChangeManager(AuthDbContext context) : IChangeManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<Result> CreateAsync(UserEntity user, ChangeField field,
        string value, CancellationToken cancellationToken = default)
    {
        var existingEntities = await context.UserChanges
            .Where(x => x.UserId == user.Id && x.Field == field)
            .ToListAsync(cancellationToken);

        var version = existingEntities.Count == 0 ? 1 : existingEntities.Max(x => x.Version) + 1;

        var entity = new UserChangesEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Field = field,
            Version = version,
            Value = value,
            CreateDate = DateTimeOffset.UtcNow
        };
        
        await context.UserChanges.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}