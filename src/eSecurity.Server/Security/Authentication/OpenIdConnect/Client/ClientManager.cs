using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Client;

public class ClientManager(AuthDbContext context) : IClientManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<List<string>> GetFrontChannelLogoutUrisAsync(SessionEntity session, 
        CancellationToken cancellationToken = default)
    {
        return await _context.ClientSessions
            .Where(x => x.SessionId == session.Id)
            .SelectMany(x => x.Client.Uris)
            .Where(x => x.Type == UriType.FrontChannelLogout)
            .Select(x => x.Uri)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<ClientEntity?> FindByIdAsync(string clientId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Clients
            .Where(c => c.Id == Guid.Parse(clientId))
            .Include(x => x.Uris)
            .Include(x => x.AllowedScopes)
            .ThenInclude(x => x.Scope)
            .Include(x => x.GrantTypes)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<ClientEntity?> FindByIdAsync(Guid id, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Clients
            .Where(c => c.Id == id)
            .Include(x => x.Uris)
            .Include(x => x.AllowedScopes)
            .ThenInclude(x => x.Scope)
            .Include(x => x.GrantTypes)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<ClientEntity?> FindByAudienceAsync(string audience, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Clients
            .Where(c => c.Audience == audience || c.Id == Guid.Parse(audience))
            .Include(x => x.Uris)
            .Include(x => x.AllowedScopes)
            .ThenInclude(x => x.Scope)
            .Include(x => x.GrantTypes)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<List<string>> GetAudiencesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Clients.Select(x => x.Audience).ToListAsync(cancellationToken);
    }

    public async ValueTask<Result> RelateAsync(ClientEntity client, SessionEntity session, 
        CancellationToken cancellationToken = default)
    {
        var entity = new ClientSessionEntity()
        {
            ClientId = client.Id,
            SessionId = session.Id
        };
        
        await _context.ClientSessions.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Results.Ok();
    }
}