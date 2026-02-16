namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Logout;

public interface ILogoutHandler
{
    public ValueTask<Result> HandleAsync(LogoutContext context, CancellationToken cancellationToken = default);
}