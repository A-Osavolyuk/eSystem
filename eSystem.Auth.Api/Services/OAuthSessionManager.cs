using eSystem.Auth.Api.Data;
using eSystem.Auth.Api.Entities;
using eSystem.Auth.Api.Interfaces;
using eSystem.Domain.Common.Results;

namespace eSystem.Auth.Api.Services;

[Injectable(typeof(IOAuthSessionManager), ServiceLifetime.Scoped)]
public class OAuthSessionManager(AuthDbContext context) : IOAuthSessionManager
{
    private readonly AuthDbContext context = context;
    
    public async ValueTask<OAuthSessionEntity?> FindAsync(Guid id, string token, CancellationToken cancellationToken = default)
    {
        return await context.OAuthSessions
            .Where(x => x.Id == id && x.Token == token)
            .Include(x => x.LinkedAccount)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(OAuthSessionEntity session, CancellationToken cancellationToken = default)
    {
        await context.OAuthSessions.AddAsync(session, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UpdateAsync(OAuthSessionEntity session, CancellationToken cancellationToken = default)
    {
        session.UpdateDate = DateTimeOffset.UtcNow;
        
        context.OAuthSessions.Update(session);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}