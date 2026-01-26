using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Authentication.Schemes;
using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Server.Security.Authentication.Handlers;

public sealed class ClientSecretPostAuthenticationSchemeOptions : AuthenticationSchemeOptions {}

public class ClientSecretPostAuthenticationHandler(
    IOptionsMonitor<ClientSecretPostAuthenticationSchemeOptions> options, 
    ILoggerFactory logger,
    IClientManager clientManager,
    UrlEncoder encoder) : AuthenticationHandler<ClientSecretPostAuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly IClientManager _clientManager = clientManager;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.HasFormContentType)
        {
            Context.Items["error"] = ErrorTypes.OAuth.InvalidClient;
            return AuthenticateResult.Fail("Unauthorized.");
        }
        
        var body = await Request.ReadFormAsync();
        if (!body.TryGetValue("client_id", out var clientId))
        {
            Context.Items["error"] = ErrorTypes.OAuth.InvalidClient;
            return AuthenticateResult.Fail("Unauthorized.");
        }
        
        var client = await _clientManager.FindByIdAsync(clientId.ToString());
        if (client is null)
        {
            Context.Items["error"] = ErrorTypes.OAuth.InvalidClient;
            return AuthenticateResult.Fail("Unauthorized.");
        }

        if (client is { ClientType: ClientType.Confidential, RequireClientSecret: true })
        {
            if (!body.TryGetValue("client_secret", out var secret) ||
                !CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(client.Secret!), 
                    Encoding.UTF8.GetBytes(secret.ToString())))
            {
                Context.Items["error"] = ErrorTypes.OAuth.InvalidClient;
                return AuthenticateResult.Fail("Unauthorized.");
            }
        }

        var claims = new List<Claim>
        {
            new("client_id", client.Id.ToString()),
            new("client_type", client.ClientType.ToString())
        };

        var identity = new ClaimsIdentity(claims, ClientSecretPostAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, ClientSecretPostAuthenticationDefaults.AuthenticationScheme);

        return AuthenticateResult.Success(ticket);
    }
    
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var error = Context.Items["error"]?.ToString();

        if (string.IsNullOrEmpty(error) || error == ErrorTypes.OAuth.ServerError)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            Response.ContentType = ContentTypes.Application.Json;
        
            await Response.WriteAsJsonAsync(new Error
            {
                Code = ErrorTypes.OAuth.ServerError, 
                Description = "Server error."
            });
        }
        else
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            Response.ContentType = ContentTypes.Application.Json;
        
            await Response.WriteAsJsonAsync(new Error
            {
                Code = error, 
                Description = "Invalid client."
            });
        }
    }
}