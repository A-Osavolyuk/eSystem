using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Logout;

public interface ILogoutStrategy<TResult> where TResult : class, new()
{
    public ValueTask<TResult> ExecuteAsync(SessionEntity session, CancellationToken cancellationToken);
}