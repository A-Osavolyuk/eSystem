using eSystem.Auth.Api.Data.Entities;
using eSystem.Core.Attributes;

namespace eSystem.Auth.Api.Security.Authorization.Permissions;

[Injectable(typeof(IPermissionManager), ServiceLifetime.Scoped)]
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

    public async ValueTask<Result> GrantAsync(UserEntity user, PermissionEntity permission,
        CancellationToken cancellationToken = default)
    {
        var hasPermission = await context.UserPermissions.AnyAsync(
            x => x.UserId == user.Id && x.PermissionId == permission.Id, cancellationToken);
        
        if (hasPermission)
        {
            return Result.Success();
        }
        
        var entity = new UserPermissionsEntity() { UserId = user.Id, PermissionId = permission!.Id };
        await context.UserPermissions.AddAsync( entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async ValueTask<Result> RevokeAsync(UserEntity user,
        PermissionEntity permissionEntity,
        CancellationToken cancellationToken = default)
    {
        var userPermission = await context.UserPermissions
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.PermissionId == permissionEntity.Id,
                cancellationToken: cancellationToken);

        if (userPermission is null)
        {
            return Results.NotFound($"User doesn't have permission {permissionEntity.Name}");
        }

        context.UserPermissions.Remove(userPermission);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}