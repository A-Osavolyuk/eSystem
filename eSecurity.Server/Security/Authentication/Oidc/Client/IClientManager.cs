using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Oidc.Client;

public interface IClientManager
{
    public ValueTask<ClientEntity?> FindByClientIdAsync(string clientId, CancellationToken cancellationToken = default);
    public ValueTask<ClientEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<ClientEntity?> FindByAudienceAsync(string audience, CancellationToken cancellationToken = default);
    public ValueTask<List<string>> GetAudiencesAsync(CancellationToken cancellationToken = default);
}