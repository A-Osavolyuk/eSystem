namespace eShop.Auth.Api.Services;

public class RoleManager(AuthDbContext context) : IRoleManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await context.Roles.AnyAsync(x => x.Name == name, cancellationToken);
    }

    public async ValueTask<List<RoleEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await context.Roles.ToListAsync(cancellationToken);
        return roles;
    }

    public async ValueTask<List<RoleEntity>> GetByUserAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var roles = await context.UserRoles
            .Where(x => x.UserId == user.Id)
            .Include(x => x.Role)
            .Select(x => x.Role)
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

    public async ValueTask DeleteAsync(RoleEntity entity, CancellationToken cancellationToken = default)
    {
        context.Roles.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask CreateAsync(RoleEntity entity, CancellationToken cancellationToken = default)
    {
        await context.Roles.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask UpdateAsync(RoleEntity entity, CancellationToken cancellationToken = default)
    {
        context.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async ValueTask<Result> UnassignRoleAsync(UserEntity user, string roleName,
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

    public async ValueTask<Result> UnassignRoleAsync(UserEntity user, RoleEntity role,
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

    public async ValueTask<Result> UnassignRolesAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var userRoles = await context.UserRoles
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);
        
        context.UserRoles.RemoveRange(userRoles);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
    
    public async ValueTask<Result> AssignRoleAsync(UserEntity user, string roleName,
        CancellationToken cancellationToken = default)
    {
        var role = await context.Roles.FirstOrDefaultAsync(x => x.Name == roleName || x.NormalizedName == roleName,
            cancellationToken);

        if (role is null)
        {
            return Results.NotFound("Role not found");
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

    public async ValueTask<Result> AssignRoleAsync(UserEntity user, RoleEntity role,
        CancellationToken cancellationToken = default)
    {
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
    
    public async ValueTask<bool> IsInRoleAsync(UserEntity user, string roleName,
        CancellationToken cancellationToken = default)
    {
        var isInRole = await context.UserRoles
            .AnyAsync(x => x.UserId == user.Id && x.Role.Name == roleName, cancellationToken: cancellationToken);

        return isInRole;
    }
}