using System.Security.Claims;
using System.Security.Cryptography;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Http.Constants;
using eSystem.Core.Security.Authentication.Schemes;
using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Server.Security.Authentication.Handlers.Basic;

public class HeaderBasicAuthenticationStrategy(IClientManager clientManager) : IBasicAuthenticationStrategy
{
    private readonly IClientManager _clientManager = clientManager;

    public bool CanExecute(HttpContext httpContext)
    {
        var header = httpContext.Request.Headers.Authorization.ToString();
        return !string.IsNullOrEmpty(header) && 
               header.StartsWith($"{AuthenticationTypes.Basic} ", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<AuthenticateResult> ExecuteAsync(HttpContext httpContext, 
        CancellationToken cancellationToken = default)
    {
        byte[] bytes;
        try
        {
            var header = httpContext.Request.Headers.Authorization.ToString();
            var base64 = header[$"{AuthenticationTypes.Basic} ".Length..];
            bytes = Convert.FromBase64String(base64);
        }
        catch (FormatException)
        {
            httpContext.Items["error"] = ErrorTypes.OAuth.InvalidClient;
            return AuthenticateResult.Fail("Unauthorized.");
        }
        
        var value = Encoding.UTF8.GetString(bytes);
        if (!value.Contains(':'))
        {
            httpContext.Items["error"] = ErrorTypes.OAuth.InvalidClient;
            return AuthenticateResult.Fail("Unauthorized.");
        }

        var valueParts = value.Split(':', 2);
        if (valueParts.Length != 2)
        {
            httpContext.Items["error"] = ErrorTypes.OAuth.InvalidClient;
            return AuthenticateResult.Fail("Unauthorized.");
        }


        var clientId = valueParts.First();
        var client = await _clientManager.FindByIdAsync(clientId, cancellationToken);
        if (client is null)
        {
            httpContext.Items["error"] = ErrorTypes.OAuth.InvalidClient;
            return AuthenticateResult.Fail("Unauthorized.");
        }

        if (client is { ClientType: ClientType.Confidential, RequireClientSecret: true })
        {
            var clientSecret = valueParts.Last();
            if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(client.Secret!),
                    Encoding.UTF8.GetBytes(clientSecret)))
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