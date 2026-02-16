using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Client;

public class ClientManager(AuthDbContext context) : IClientManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<List<ClientEntity>> GetClientsAsync(SessionEntity session,
        CancellationToken cancellationToken = default)
    {
        return await _context.ClientSessions
            .Where(x => x.SessionId == session.Id)
            .Include(x => x.Client.Uris)
            .Include(x => x.Client.AllowedScopes)
            .ThenInclude(x => x.Scope)
            .Include(x => x.Client.GrantTypes)
            .ThenInclude(x => x.Grant)
            .Include(x => x.Client.PairwiseSubjects)
            .Include(x => x.Client.Audiences)
            .Select(x => x.Client)
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
            .ThenInclude(x => x.Grant)
            .Include(x => x.PairwiseSubjects)
            .Include(x => x.Audiences)
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
            .ThenInclude(x => x.Grant)
            .Include(x => x.PairwiseSubjects)
            .Include(x => x.Audiences)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<List<string>> GetAudiencesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.ClientAudiences
            .Select(x => x.Audience)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(ClientEntity entity, CancellationToken cancellationToken = default)
    {
        await _context.Clients.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }

    public async ValueTask<Result> RelateAsync(ClientEntity client, SessionEntity session,
        CancellationToken cancellationToken = default)
    {
        if (!await _context.ClientSessions.AnyAsync(x => x.SessionId == session.Id && 
                                                         x.ClientId == client.Id, cancellationToken))
        {
            var entity = new ClientSessionEntity()
            {
                ClientId = client.Id,
                SessionId = session.Id
            };

            await _context.ClientSessions.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        
        return Results.Ok();
    }
}