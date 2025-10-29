using eSystem.Auth.Api.Data.Entities;
using eSystem.Core.Attributes;

namespace eSystem.Auth.Api.Services;

[Injectable(typeof(IRoleManager), ServiceLifetime.Scoped)]
public sealed class RoleManager(AuthDbContext context) : IRoleManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<List<RoleEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await context.Roles
            .Include(x => x.Permissions)
            .ThenInclude(x => x.Permission)
            .ToListAsync(cancellationToken);
        
        return roles;
    }

    public async ValueTask<RoleEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var role = await context.Roles
            .Where(x => x.Name == name)
            .Include(x => x.Permissions)
            .ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(cancellationToken);
        
        return role;
    }
    
    public async ValueTask<RoleEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await context.Roles            
            .Where(x => x.Id == id)
            .Include(x => x.Permissions)
            .ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(cancellationToken);
        
        return role;
    }

    public async ValueTask<Result> DeleteAsync(RoleEntity entity, CancellationToken cancellationToken = default)
    {
        context.Roles.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> CreateAsync(RoleEntity entity, CancellationToken cancellationToken = default)
    {
        await context.Roles.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> UpdateAsync(RoleEntity entity, CancellationToken cancellationToken = default)
    {
        context.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return  Result.Success();
    }
    
    public async ValueTask<Result> UnassignAsync(UserEntity user, string roleName,
        CancellationToken cancellationToken = default)
    {
        var role = await context.UserRoles
            .FirstOrDefaultAsync(x => x.UserId == user.Id
                                      && (x.Role.Name == roleName || x.Role.NormalizedName == roleName),
                cancellationToken);

        if (role is null)
        {
            return Results.NotFound("User not in role");
        }

        context.UserRoles.Remove(role);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UnassignAsync(UserEntity user, RoleEntity role,
        CancellationToken cancellationToken = default)
    {
        var userRole = await context.UserRoles
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.RoleId == role.Id, cancellationToken);
        
        if (userRole is null)
        {
            return Results.NotFound("User not in role");
        }
        
        context.UserRoles.Remove(userRole);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UnassignAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var userRoles = await context.UserRoles
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);
        
        context.UserRoles.RemoveRange(userRoles);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> AssignAsync(UserEntity user, RoleEntity role,
        CancellationToken cancellationToken = default)
    {
        var hasRole = await context.UserRoles.AnyAsync(
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

        await context.UserRoles.AddAsync(userRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}