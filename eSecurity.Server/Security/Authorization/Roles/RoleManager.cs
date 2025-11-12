using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authorization.Roles;

public sealed class RoleManager(AuthDbContext context) : IRoleManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<List<RoleEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _context.Roles
            .Include(x => x.Permissions)
            .ThenInclude(x => x.Permission)
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
    
    public async ValueTask<RoleEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles            
            .Where(x => x.Id == id)
            .Include(x => x.Permissions)
            .ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(cancellationToken);
        
        return role;
    }

    public async ValueTask<Result> DeleteAsync(RoleEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Roles.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> CreateAsync(RoleEntity entity, CancellationToken cancellationToken = default)
    {
        await _context.Roles.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> UpdateAsync(RoleEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return  Result.Success();
    }
    
    public async ValueTask<Result> UnassignAsync(UserEntity user, string roleName,
        CancellationToken cancellationToken = default)
    {
        var role = await _context.UserRoles
            .FirstOrDefaultAsync(x => x.UserId == user.Id
                                      && (x.Role.Name == roleName || x.Role.NormalizedName == roleName),
                cancellationToken);

        if (role is null)
        {
            return Results.NotFound("User not in role");
        }

        _context.UserRoles.Remove(role);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UnassignAsync(UserEntity user, RoleEntity role,
        CancellationToken cancellationToken = default)
    {
        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.RoleId == role.Id, cancellationToken);
        
        if (userRole is null)
        {
            return Results.NotFound("User not in role");
        }
        
        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UnassignAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var userRoles = await _context.UserRoles
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);
        
        _context.UserRoles.RemoveRange(userRoles);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> AssignAsync(UserEntity user, RoleEntity role,
        CancellationToken cancellationToken = default)
    {
        var hasRole = await _context.UserRoles.AnyAsync(
            x => x.UserId == user.Id && x.RoleId == role.Id, cancellationToken);

        if (hasRole)
        {
            return Result.Success();
        }
        
        var userRole = new UserRoleEntity()
        {
            UserId = user.Id,
            RoleId = role.Id,
            CreateDate = DateTime.UtcNow
        };

        await _context.UserRoles.AddAsync(userRole, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}