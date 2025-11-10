using eSecurity.Data.Entities;

namespace eSecurity.Security.Authorization.Consents;

public class ConsentManager(AuthDbContext context) : IConsentManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<ConsentEntity?> FindByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return await context.Consents.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async ValueTask<ConsentEntity?> FindAsync(UserEntity user, ClientEntity client,
        CancellationToken cancellationToken = default)
    {
        return await context.Consents.FirstOrDefaultAsync(
            c => c.UserId == user.Id && c.ClientId == client.Id, cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(UserEntity user, ClientEntity client, 
        CancellationToken cancellationToken = default)
    {
        var consent = new ConsentEntity()
        {
            UserId = user.Id,
            ClientId = client.Id,
            CreateDate = DateTimeOffset.UtcNow
        };
        
        await context.Consents.AddAsync(consent, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> GrantAsync(ConsentEntity consent, ScopeEntity scope,
        CancellationToken cancellationToken = default)
    {
        var grantedScope = new GrantedScopeEntity()
        {
            ConsentId = consent.Id,
            ScopeId = scope.Id,
            CreateDate = DateTimeOffset.UtcNow
        };

        await context.GrantedScopes.AddAsync(grantedScope, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> RevokeAsync(ConsentEntity consent, ScopeEntity scope,
        CancellationToken cancellationToken = default)
    {
        var grantedScope = await context.GrantedScopes.FirstOrDefaultAsync(
            x => x.ConsentId == consent.Id && x.ScopeId == scope.Id, cancellationToken);

        if (grantedScope is null) return Results.NotFound("Scope is not granted.");

        context.GrantedScopes.Remove(grantedScope);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}