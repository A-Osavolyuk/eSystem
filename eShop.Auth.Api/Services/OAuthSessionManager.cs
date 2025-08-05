namespace eShop.Auth.Api.Services;

[Injectable(typeof(IOAuthSessionManager), ServiceLifetime.Scoped)]
public class OAuthSessionManager(AuthDbContext context) : IOAuthSessionManager
{
    private readonly AuthDbContext context = context;
    
    public async ValueTask<OAuthSessionEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.OAuthSessions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity;
    }

    public async ValueTask<Result> CreateAsync(OAuthSessionEntity session, CancellationToken cancellationToken = default)
    {
        await  context.OAuthSessions.AddAsync(session, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}