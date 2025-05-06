namespace eShop.Auth.Api.Services;

public class RoleManager(AuthDbContext context) : IRoleManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<List<RoleEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await context.Roles.ToListAsync(cancellationToken);
        return roles;
    }

    public async ValueTask<List<RoleEntity>> GetAllAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var roles = await context.UserRoles
            .Where(x => x.UserId == user.Id)
            .Include(x => x.Role)
            .Select(x => x.Role)
            .ToListAsync(cancellationToken);
        
        return roles;
    }
}