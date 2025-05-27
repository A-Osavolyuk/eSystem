using eShop.Application.Attributes;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(IPermissionManager), Lifetime = ServiceLifetime.Scoped)]
public sealed class PermissionManager(AuthDbContext context) : IPermissionManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<PermissionEntity?> FindByNameAsync(string name,
        CancellationToken cancellationToken = default)
    {
        var permission = await context.Permissions
            .SingleOrDefaultAsync(x => x.Name == name, cancellationToken);

        return permission;
    }

    public async ValueTask<PermissionEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var permission = await context.Permissions
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        return permission;
    }

    public async ValueTask<List<PermissionEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await context.Permissions
            .ToListAsync(cancellationToken);

        return permissions;
    }

    public async ValueTask<List<PermissionEntity>> GetByUserAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        var permissions = await context.UserPermissions
            .Where(x => x.UserId == user.Id)
            .Include(x => x.Permission)
            .Select(x => x.Permission)
            .ToListAsync(cancellationToken);

        return permissions;
    }

    public async ValueTask<Result> GrantAsync(UserEntity user, List<PermissionEntity> collection,
        CancellationToken cancellationToken = default)
    {
        var permissions = collection.ToList();

        if (!permissions.Any())
        {
            return Results.NotFound("Cannot add permissions. Empty permission list.");
        }

        var entities = permissions.Select(x =>
            new UserPermissionsEntity()
            {
                UserId = user.Id,
                PermissionId = x.Id
            });

        await context.UserPermissions.AddRangeAsync(entities, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async ValueTask<Result> GrantAsync(UserEntity user, PermissionEntity permission,
        CancellationToken cancellationToken = default)
    {
        var entity = new UserPermissionsEntity() { UserId = user.Id, PermissionId = permission!.Id };
        await context.UserPermissions.AddAsync( entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async ValueTask<Result> RevokeAsync(UserEntity userEntity,
        PermissionEntity permissionEntity,
        CancellationToken cancellationToken = default)
    {
        var userPermission = await context.UserPermissions
            .FirstOrDefaultAsync(x => x.UserId == userEntity.Id && x.PermissionId == permissionEntity.Id,
                cancellationToken: cancellationToken);

        if (userPermission is null)
        {
            return Results.NotFound($"Cannot find permission {permissionEntity.Name} for user with ID {userEntity.Id}");
        }

        context.UserPermissions.Remove(userPermission);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> RevokeAsync(UserEntity userEntity,
        CancellationToken cancellationToken = default)
    {
        var userPermissions = await context.UserPermissions
            .Where(x => x.UserId == userEntity.Id)
            .ToListAsync(cancellationToken: cancellationToken);

        if (userPermissions.Any())
        {
            context.UserPermissions.RemoveRange(userPermissions);
            await context.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }

    public async ValueTask<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await context.Permissions
            .AsNoTracking()
            .AnyAsync(x => x.Name == name, cancellationToken: cancellationToken);
    }

    public async ValueTask<bool> HasAsync(UserEntity userEntity, string name,
        CancellationToken cancellationToken = default)
    {
        var permission = await context.Permissions
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Name == name, cancellationToken: cancellationToken);

        if (permission is null)
        {
            return false;
        }

        var hasUserPermission = await context.UserPermissions
            .AsNoTracking()
            .AnyAsync(x => x.UserId == userEntity.Id && x.PermissionId == permission.Id,
                cancellationToken: cancellationToken);

        return hasUserPermission;
    }
}