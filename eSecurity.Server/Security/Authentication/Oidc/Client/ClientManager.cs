using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Oidc.Client;

public class ClientManager(AuthDbContext context) : IClientManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<ClientEntity?> FindByIdAsync(string clientId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Clients
            .Where(c => c.Id == Guid.Parse(clientId))
            .Include(x => x.RedirectUris)
            .Include(x => x.PostLogoutRedirectUris)
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
            .Include(x => x.RedirectUris)
            .Include(x => x.PostLogoutRedirectUris)
            .Include(x => x.AllowedScopes)
            .ThenInclude(x => x.Scope)
            .Include(x => x.GrantTypes)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<ClientEntity?> FindByAudienceAsync(string audience, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Clients
            .Where(c => c.Audience == audience)
            .Include(x => x.RedirectUris)
            .Include(x => x.PostLogoutRedirectUris)
            .Include(x => x.AllowedScopes)
            .ThenInclude(x => x.Scope)
            .Include(x => x.GrantTypes)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<List<string>> GetAudiencesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Clients.Select(x => x.Audience).ToListAsync(cancellationToken);
    }
}