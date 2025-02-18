namespace eShop.Auth.Api.Services;

internal sealed class PermissionManager(AuthDbContext context) : IPermissionManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<PermissionEntity?> FindPermissionAsync(string name)
    {
        var permission = await context.Permissions.AsNoTracking().SingleOrDefaultAsync(x => x.Name == name);
        return permission;
    }

    public async ValueTask<List<PermissionEntity>> GetPermissionsAsync()
    {
        var permissions = await context.Permissions.AsNoTracking().ToListAsync();
        return permissions;
    }

    public async ValueTask<List<string>> GetUserPermissionsAsync(AppUser user)
    {
        var permissions = await context.UserPermissions.AsNoTracking()
            .Where(x => x.UserId == Guid.Parse(user.Id))
            .ToListAsync();
        var result = new List<string>();

        if (!permissions.Any())
        {
            return result;
        }

        foreach (var permission in permissions)
        {
            var permissionName = (await context.Permissions.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == permission.Id))!.Name;
            result.Add(permissionName);
        }

        return result;
    }

    public async ValueTask<IdentityResult> IssuePermissionsAsync(AppUser user, IList<string> permissions)
    {
        if (!permissions.Any())
        {
            return IdentityResult.Failed(new IdentityError()
                { Code = "400", Description = "Cannot add permissions. Empty permission list." });
        }

        foreach (var permission in permissions)
        {
            var permissionId =
                (await context.Permissions.AsNoTracking().SingleOrDefaultAsync(x => x.Name == permission))!.Id;
            await context.UserPermissions.AddAsync(new() { UserId = Guid.Parse(user.Id), Id = permissionId });
        }

        await context.SaveChangesAsync();
        return IdentityResult.Success;
    }

    public async ValueTask<IdentityResult> IssuePermissionAsync(AppUser user, string permission)
    {
        var entity = await context.Permissions.AsNoTracking().SingleOrDefaultAsync(x => x.Name == permission);
        await context.UserPermissions.AddAsync(new() { UserId = Guid.Parse(user.Id), Id = entity!.Id });
        await context.SaveChangesAsync();
        return IdentityResult.Success;
    }

    public async ValueTask<IdentityResult> RemoveFromPermissionAsync(AppUser user, PermissionEntity permissionEntity)
    {
        var userPermission = await context.UserPermissions.AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserId == Guid.Parse(user.Id) && x.Id == permissionEntity.Id);

        if (userPermission is null)
        {
            return IdentityResult.Failed(
                new IdentityError()
                {
                    Code = "404",
                    Description = string.Format("Cannot find permission {0} for user with ID {1}",
                        permissionEntity.Name, Guid.Parse(user.Id))
                });
        }

        context.UserPermissions.Remove(userPermission);
        await context.SaveChangesAsync();

        return IdentityResult.Success;
    }

    public async ValueTask<IdentityResult> RemoveFromPermissionsAsync(AppUser user)
    {
        var userPermissions = await context.UserPermissions
            .AsNoTracking()
            .Where(x => x.UserId == Guid.Parse(user.Id))
            .ToListAsync();

        if (userPermissions.Any())
        {
            context.UserPermissions.RemoveRange(userPermissions);
            await context.SaveChangesAsync();
        }

        return IdentityResult.Success;
    }

    public async ValueTask<bool> ExistsAsync(string name)
    {
        return await context.Permissions.AsNoTracking().AnyAsync(x => x.Name == name);
    }

    public async ValueTask<bool> HasPermissionAsync(AppUser user, string name)
    {
        var permission = await context.Permissions.AsNoTracking().SingleOrDefaultAsync(x => x.Name == name);

        if (permission is null)
        {
            return false;
        }

        var hasUserPermission = await context.UserPermissions.AsNoTracking()
            .AnyAsync(x => x.UserId == Guid.Parse(user.Id) && x.Id == permission.Id);

        if (hasUserPermission)
        {
            return true;
        }

        return false;
    }
}