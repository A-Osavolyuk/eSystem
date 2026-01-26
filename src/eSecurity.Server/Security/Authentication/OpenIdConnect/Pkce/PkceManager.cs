using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Pkce;

public class PkceManager(AuthDbContext context) : IPkceManager
{
    private readonly AuthDbContext _context = context;
    
    public async ValueTask<bool> IsVerified(Guid clientId, Guid sessionId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.ClientPkceStates.AnyAsync(
            x => x.ClientId == clientId && x.SessionId == sessionId, cancellationToken);
    }
    
    public async ValueTask<Result> CreateAsync(ClientPkceStateEntity entity,
        CancellationToken cancellationToken = default)
    {
        await _context.ClientPkceStates.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Results.Ok();
    }
}