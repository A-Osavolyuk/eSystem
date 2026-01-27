namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Authorization;

public interface IOpenIdConnectAuthorizationHandler
{
    public Task HandleAsync(AuthorizationContext context);
}