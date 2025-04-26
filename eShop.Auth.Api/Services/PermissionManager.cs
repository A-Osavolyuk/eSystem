namespace eShop.Auth.Api.Services;

internal sealed class PermissionManager(AuthDbContext context) : IPermissionManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<PermissionEntity?> FindByNameAsync(string name,
        CancellationToken cancellationToken = default)
    {
        var permission = await context.Permissions
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Name == name, cancellationToken);

        return permission;
    }

    public async ValueTask<List<PermissionEntity>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await context.Permissions
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return permissions;
    }

    public async ValueTask<List<string>> GetUserPermissionsAsync(UserEntity userEntity,
        CancellationToken cancellationToken = default)
    {
        var permissions = await context.UserPermissions
            .AsNoTracking()
            .Where(x => x.UserId == userEntity.Id)
            .ToListAsync(cancellationToken: cancellationToken);
        var result = new List<string>();

        if (!permissions.Any())
        {
            return result;
        }

        foreach (var permission in permissions)
        {
            var permissionName = (await context.Permissions
                .AsNoTracking()
                .Where(x => x.Id == permission.Id)
                .Select(x => x.Name)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken));

            result.Add(permissionName!);
        }

        return result;
    }

    public async ValueTask<IdentityResult> IssuePermissionsAsync(UserEntity userEntity, IEnumerable<string> collection,
        CancellationToken cancellationToken = default)
    {
        var permissions = collection.ToList();

        if (!permissions.Any())
        {
            return IdentityResult.Failed(new IdentityError()
            {
                Code = "400",
                Description = "Cannot add permissions. Empty permission list."
            });
        }

        foreach (var permission in permissions)
        {
            var entity = await context.Permissions
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Name == permission, cancellationToken: cancellationToken);

            await context.UserPermissions.AddAsync(new() { UserId = userEntity.Id, Id = entity!.Id }, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public async ValueTask<IdentityResult> IssuePermissionAsync(UserEntity userEntity, string permission,
        CancellationToken cancellationToken = default)
    {
        var entity = await context.Permissions
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Name == permission, cancellationToken: cancellationToken);

        await context.UserPermissions.AddAsync(new() { UserId = userEntity.Id, Id = entity!.Id }, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public async ValueTask<IdentityResult> RemoveFromPermissionAsync(UserEntity userEntity, PermissionEntity permissionEntity,
        CancellationToken cancellationToken = default)
    {
        var userPermission = await context.UserPermissions
            .FirstOrDefaultAsync(x => x.UserId == userEntity.Id && x.Id == permissionEntity.Id, cancellationToken: cancellationToken);

        if (userPermission is null)
        {
            return IdentityResult.Failed(
                new IdentityError()
                {
                    Code = "404",
                    Description = string.Format("Cannot find permission {0} for user with ID {1}",
                        permissionEntity.Name, userEntity.Id)
                });
        }

        context.UserPermissions.Remove(userPermission);
        await context.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }

    public async ValueTask<IdentityResult> RemoveFromPermissionsAsync(UserEntity userEntity, CancellationToken cancellationToken = default)
    {
        var userPermissions = await context.UserPermissions
            .Where(x => x.UserId == userEntity.Id)
            .ToListAsync(cancellationToken: cancellationToken);

        if (userPermissions.Any())
        {
            context.UserPermissions.RemoveRange(userPermissions);
            await context.SaveChangesAsync(cancellationToken);
        }

        return IdentityResult.Success;
    }

    public async ValueTask<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await context.Permissions
            .AsNoTracking()
            .AnyAsync(x => x.Name == name, cancellationToken: cancellationToken);
    }

    public async ValueTask<bool> HasPermissionAsync(UserEntity userEntity, string name,
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
            .AnyAsync(x => x.UserId == userEntity.Id && x.Id == permission.Id, cancellationToken: cancellationToken);

        return hasUserPermission;
    }
}