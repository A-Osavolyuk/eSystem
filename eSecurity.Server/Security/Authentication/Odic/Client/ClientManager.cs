using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Odic.Client;

public class ClientManager(AuthDbContext context) : IClientManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<ClientEntity?> FindByClientIdAsync(string clientId, CancellationToken cancellationToken = default)
    {
        return await context.Clients
            .Where(c => c.ClientId == clientId)
            .Include(x => x.RedirectUris)
            .Include(x => x.PostLogoutRedirectUris)
            .Include(x => x.AllowedScopes)
            .ThenInclude(x => x.Scope)
            .Include(x => x.GrantTypes)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<ClientEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Clients
            .Where(c => c.Id == id)
            .Include(x => x.RedirectUris)
            .Include(x => x.PostLogoutRedirectUris)
            .Include(x => x.AllowedScopes)
            .ThenInclude(x => x.Scope)
            .Include(x => x.GrantTypes)
            .FirstOrDefaultAsync(cancellationToken);
    }
}