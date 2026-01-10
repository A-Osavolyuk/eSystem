using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authorization.Permissions;

public sealed class PermissionManager(AuthDbContext context) : IPermissionManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<List<UserPermissionsEntity>> GetAllAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserPermissions
            .Where(x => x.UserId == user.Id)
            .Include(x => x.Permission)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<Result> GrantAsync(UserEntity user, PermissionEntity permission,
        CancellationToken cancellationToken = default)
    {
        var hasPermission = await _context.UserPermissions.AnyAsync(
            x => x.UserId == user.Id && x.PermissionId == permission.Id, cancellationToken);

        if (hasPermission)
        {
            return Results.Ok();
        }

        var entity = new UserPermissionsEntity() { UserId = user.Id, PermissionId = permission!.Id };
        await _context.UserPermissions.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Results.Ok();
    }
}