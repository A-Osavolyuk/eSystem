using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Server.Security.Authentication.Handlers.Basic;

public interface IBasicAuthenticationStrategy
{
    public bool CanExecute(HttpContext httpContext);
    public Task<AuthenticateResult> ExecuteAsync(HttpContext httpContext, CancellationToken cancellationToken = default);
}