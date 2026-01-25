using System.Security.Claims;
using System.Security.Cryptography;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Http.Constants;
using eSystem.Core.Security.Authentication.Schemes;
using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Server.Security.Authentication.Handlers.Basic;

public class FormDataBasicAuthenticationStrategy(IClientManager clientManager) : IBasicAuthenticationStrategy
{
    private readonly IClientManager _clientManager = clientManager;

    public bool CanExecute(HttpContext httpContext) => httpContext.Request.HasFormContentType;

    public async Task<AuthenticateResult> ExecuteAsync(HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        var body = await httpContext.Request.ReadFormAsync(cancellationToken);
        if (!body.TryGetValue("client_id", out var clientId))
        {
            httpContext.Items["error"] = ErrorTypes.OAuth.InvalidClient;
            return AuthenticateResult.Fail("Unauthorized.");
        }

        var client = await _clientManager.FindByIdAsync(clientId.ToString(), cancellationToken);
        if (client is null)
        {
            httpContext.Items["error"] = ErrorTypes.OAuth.InvalidClient;
            return AuthenticateResult.Fail("Unauthorized.");
        }

        if (client is { ClientType: ClientType.Confidential, RequireClientSecret: true })
        {
            if (!body.TryGetValue("client_secret", out var secret) ||
                !CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(client.Secret!), 
                    Encoding.UTF8.GetBytes(secret.ToString())))
            {
                httpContext.Items["error"] = ErrorTypes.OAuth.InvalidClient;
                return AuthenticateResult.Fail("Unauthorized.");
            }
        }

        var claims = new List<Claim>
        {
            new("client_id", client.Id.ToString()),
            new("client_type", client.ClientType.ToString())
        };

        var identity = new ClaimsIdentity(claims, BasicAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, BasicAuthenticationDefaults.AuthenticationScheme);

        return AuthenticateResult.Success(ticket);
    }
}