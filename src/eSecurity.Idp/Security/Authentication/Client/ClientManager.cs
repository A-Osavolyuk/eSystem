using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authentication.Client;

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
            .Include(x => x.ResponseTypes)
            .ThenInclude(x => x.ResponseType)
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
            .Include(x => x.ResponseTypes)
            .ThenInclude(x => x.ResponseType)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<Result> RelateAsync(ClientEntity client, SessionEntity session,
        CancellationToken cancellationToken = default)
    {
        if (!await _context.ClientSessions.AnyAsync(x => x.SessionId == session.Id && 
                                                         x.ClientId == client.Id, cancellationToken))
        {
            var entity = new ClientSessionEntity
            {
                ClientId = client.Id,
                SessionId = session.Id
            };

            await _context.ClientSessions.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}