namespace eSystem.Auth.Api.Security.Authentication.SSO;

public interface IClientManager
{
    public ValueTask<ClientEntity?> FindByClientIdAsync(string clientId, CancellationToken cancellationToken = default);
    public ValueTask<ClientEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}