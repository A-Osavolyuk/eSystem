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
        var role = await context.Roles.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        return role;
    }
    
    public async ValueTask<RoleEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await context.Roles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
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
}