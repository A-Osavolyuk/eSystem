using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Client;

public interface IClientManager
{
    public ValueTask<ClientEntity?> FindByIdAsync(string clientId, CancellationToken cancellationToken = default);
    public ValueTask<ClientEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<ClientEntity?> FindByAudienceAsync(string audience, CancellationToken cancellationToken = default);
    public ValueTask<List<string>> GetAudiencesAsync(CancellationToken cancellationToken = default);
}