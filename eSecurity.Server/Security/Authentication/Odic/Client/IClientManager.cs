using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Odic.Client;

public interface IClientManager
{
    public ValueTask<ClientEntity?> FindByClientIdAsync(string clientId, CancellationToken cancellationToken = default);
    public ValueTask<ClientEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}