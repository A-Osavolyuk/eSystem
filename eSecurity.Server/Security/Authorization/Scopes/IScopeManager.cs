using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authorization.Scopes;

public interface IScopeManager
{
    public ValueTask<ScopeEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}