namespace eShop.Auth.Api.Services;

[Injectable(typeof(ILoginMethodManager), ServiceLifetime.Scoped)]
public class LoginMethodManager(AuthDbContext context) : ILoginMethodManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<List<LoginMethodEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await context.LoginMethods.ToListAsync(cancellationToken);
        return entities;
    }

    public async ValueTask<LoginMethodEntity?> FindAsync(LoginType type, CancellationToken cancellationToken = default)
    {
        var entity = await context.LoginMethods.FirstOrDefaultAsync(
            x => x.Type == type, cancellationToken);
        
        return entity;
    }
}