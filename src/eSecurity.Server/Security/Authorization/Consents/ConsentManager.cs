using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.Consents;

public class ConsentManager(AuthDbContext context) : IConsentManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<ConsentEntity?> FindByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Consents.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async ValueTask<ConsentEntity?> FindAsync(UserEntity user, ClientEntity client,
        CancellationToken cancellationToken = default)
    {
        return await _context.Consents
            .Include(x => x.GrantedScopes)
            .ThenInclude(x => x.Scope)
            .FirstOrDefaultAsync(c => c.UserId == user.Id && c.ClientId == client.Id, cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(ConsentEntity consent, CancellationToken cancellationToken = default)
    {
        await _context.Consents.AddAsync(consent, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }

    public async ValueTask<Result> GrantAsync(ConsentEntity consent, ScopeEntity scope,
        CancellationToken cancellationToken = default)
    {
        var grantedScope = new GrantedScopeEntity
        {
            ConsentId = consent.Id,
            ScopeId = scope.Id
        };

        await _context.GrantedScopes.AddAsync(grantedScope, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> RevokeAsync(ConsentEntity consent, ScopeEntity scope,
        CancellationToken cancellationToken = default)
    {
        var grantedScope = await _context.GrantedScopes.FirstOrDefaultAsync(
            x => x.ConsentId == consent.Id && x.ScopeId == scope.Id, cancellationToken);

        if (grantedScope is null) return Results.NotFound("Scope is not granted.");

        _context.GrantedScopes.Remove(grantedScope);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }
}