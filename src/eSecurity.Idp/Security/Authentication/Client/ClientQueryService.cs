using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authentication.Client;

public sealed class ClientQueryService(AuthDbContext context) : IClientQueryService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<List<ClientEntity>> ListBySessionAsync(Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ClientSessions
            .Where(x => x.SessionId == sessionId)
            .Select(x => x.Client)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<ClientEntity?> GetByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return await GetByIdInternalAsync(id, cancellationToken);
    }

    public async ValueTask<ClientEntity?> GetByIdAsync(string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        return await GetByIdInternalAsync(Guid.Parse(id), cancellationToken);
    }

    public async ValueTask<List<ClientResponseTypeEntity>> GetSupportedResponseTypesAsync(ClientEntity client,
        CancellationToken cancellationToken = default)
    {
        return await _context.ClientResponseTypes
            .Where(x => x.ClientId == client.Id)
            .Include(x => x.ResponseType)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<List<ClientTokenAuthMethodEntity>> GetSupportedTokenAuthMethodsAsync(ClientEntity client,
        CancellationToken cancellationToken = default)
    {
        return await _context.ClientTokenAuthMethods
            .Where(x => x.ClientId == client.Id)
            .Include(x => x.Method)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<List<ClientAllowedScopeEntity>> GetAllowedScopesAsync(ClientEntity client,
        CancellationToken cancellationToken = default)
    {
        return await _context.ClientAllowedScopes
            .Where(x => x.ClientId == client.Id)
            .Include(x => x.Scope)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<List<ClientGrantTypeEntity>> GetSupportedGrantTypesAsync(ClientEntity client,
        CancellationToken cancellationToken = default)
    {
        return await _context.ClientGrantTypes
            .Where(x => x.ClientId == client.Id)
            .Include(x => x.Grant)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<List<ClientAudienceEntity>> GetSupportedAudiencesAsync(ClientEntity client,
        CancellationToken cancellationToken = default)
    {
        return await _context.ClientAudiences
            .Where(x => x.ClientId == client.Id)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<List<ClientUriEntity>> GetUrisAsync(ClientEntity client, 
        CancellationToken cancellationToken = default)
    {
        return await _context.ClientUris
            .Where(x => x.ClientId == client.Id)
            .ToListAsync(cancellationToken);
    }

    private async ValueTask<ClientEntity?> GetByIdInternalAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Clients.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}