using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authorization.Roles;

public sealed class RoleManager(AuthDbContext context) : IRoleManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<List<UserRoleEntity>> GetAllAsync(UserEntity user, 
        CancellationToken cancellationToken = default)
    {
        var roles = await _context.UserRoles
            .Where(x => x.UserId == user.Id)
            .Include(x => x.Role)
            .ToListAsync(cancellationToken);
        
        return roles;
    }

    public async ValueTask<RoleEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles
            .Where(x => x.Name == name)
            .Include(x => x.Permissions)
            .ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(cancellationToken);
        
        return role;
    }

    public async ValueTask<Result> AssignAsync(UserEntity user, RoleEntity role,
        CancellationToken cancellationToken = default)
    {
        var hasRole = await _context.UserRoles.AnyAsync(
            x => x.UserId == user.Id && x.RoleId == role.Id, cancellationToken);

        if (hasRole)
        {
            return Results.Ok();
        }
        
        var userRole = new UserRoleEntity()
        {
            UserId = user.Id,
            RoleId = role.Id,
            CreateDate = DateTime.UtcNow
        };

        await _context.UserRoles.AddAsync(userRole, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}