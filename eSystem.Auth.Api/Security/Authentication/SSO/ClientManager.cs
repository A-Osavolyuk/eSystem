namespace eSystem.Auth.Api.Security.Authentication.SSO;

public class ClientManager(AuthDbContext context) : IClientManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<ClientEntity?> FindByClientIdAsync(string clientId, CancellationToken cancellationToken = default)
    {
        return await context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId, cancellationToken);
    }

    public async ValueTask<ClientEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Clients.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}