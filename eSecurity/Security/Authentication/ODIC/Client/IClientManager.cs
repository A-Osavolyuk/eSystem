using eSecurity.Data.Entities;

namespace eSecurity.Security.Authentication.ODIC.Client;

public interface IClientManager
{
    public ValueTask<ClientEntity?> FindByClientIdAsync(string clientId, CancellationToken cancellationToken = default);
    public ValueTask<ClientEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}