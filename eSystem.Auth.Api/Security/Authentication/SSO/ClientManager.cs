namespace eSystem.Auth.Api.Security.Authentication.SSO;

public class ClientManager(AuthDbContext context) : IClientManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<ClientEntity?> FindByClientIdAsync(string clientId, CancellationToken cancellationToken = default)
    {
        return await context.Clients
            .Where(c => c.ClientId == clientId)
            .Include(x => x.RedirectUris)
            .Include(x => x.AllowedScopes)
            .Include(x => x.GrantTypes)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<ClientEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Clients
            .Where(c => c.Id == id)
            .Include(x => x.RedirectUris)
            .Include(x => x.AllowedScopes)
            .Include(x => x.GrantTypes)
            .FirstOrDefaultAsync(cancellationToken);
    }
}