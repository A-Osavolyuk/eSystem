using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authentication.OpenIdConnect.Logout;

public interface ILogoutStrategy<TResult>
{
    public ValueTask<TResult> ExecuteAsync(SessionEntity session, CancellationToken cancellationToken);
}