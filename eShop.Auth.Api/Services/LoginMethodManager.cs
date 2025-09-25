namespace eShop.Auth.Api.Services;

[Injectable(typeof(ILoginMethodManager), ServiceLifetime.Scoped)]
public class LoginMethodManager(AuthDbContext context) : ILoginMethodManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<List<LoginMethodEntity>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var entities = await context.LoginMethods.ToListAsync(cancellationToken);
        return entities;
    }

    public async ValueTask<LoginMethodEntity?> FindAsync(LoginType type, 
        CancellationToken cancellationToken = default)
    {
        var entity = await context.LoginMethods.FirstOrDefaultAsync(
            x => x.Type == type, cancellationToken);
        
        return entity;
    }

    public async ValueTask<Result> CreateAsync(UserLoginMethodEntity entity, 
        CancellationToken cancellationToken = default)
    {
        await context.UserLoginMethods.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> RemoveAsync(UserLoginMethodEntity entity, 
        CancellationToken cancellationToken = default)
    {
        context.UserLoginMethods.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}