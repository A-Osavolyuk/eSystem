using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Authentication.Schemes;
using eSystem.Core.Security.Identity.Claims;
using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Server.Security.Authentication.Handlers;

public sealed class ClientSecretBasicAuthenticationSchemeOptions : AuthenticationSchemeOptions {}

public class ClientSecretBasicAuthenticationHandler(
    IOptionsMonitor<ClientSecretBasicAuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    IClientManager clientManager,
    UrlEncoder encoder) : AuthenticationHandler<ClientSecretBasicAuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly IClientManager _clientManager = clientManager;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(header) || 
            !header.StartsWith($"{AuthenticationTypes.Basic} ", StringComparison.OrdinalIgnoreCase))
        {
            Context.Items["error"] = ErrorTypes.OAuth.InvalidClient;
            return AuthenticateResult.Fail("Unauthorized.");
        }
        
        byte[] bytes;
        try
        {
            var base64 = header[$"{AuthenticationTypes.Basic} ".Length..];
            bytes = Convert.FromBase64String(base64);
        }
        catch (FormatException)
        {
            Context.Items["error"] = ErrorTypes.OAuth.InvalidClient;
            return AuthenticateResult.Fail("Unauthorized.");
        }
        
        var value = Encoding.UTF8.GetString(bytes);
        if (!value.Contains(':'))
        {
            Context.Items["error"] = ErrorTypes.OAuth.InvalidClient;
            return AuthenticateResult.Fail("Unauthorized.");
        }

        var valueParts = value.Split(':', 2);
        if (valueParts.Length != 2)
        {
            Context.Items["error"] = ErrorTypes.OAuth.InvalidClient;
            return AuthenticateResult.Fail("Unauthorized.");
        }


        var clientId = valueParts.First();
        var client = await _clientManager.FindByIdAsync(clientId);
        if (client is null)
        {
            Context.Items["error"] = ErrorTypes.OAuth.InvalidClient;
            return AuthenticateResult.Fail("Unauthorized.");
        }
        
        if (client is { ClientType: ClientType.Confidential, RequireClientSecret: true })
        {
            var clientSecret = valueParts.Last();
            if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(client.Secret!),
                    Encoding.UTF8.GetBytes(clientSecret)))
            {
                Context.Items["error"] = ErrorTypes.OAuth.InvalidClient;
                return AuthenticateResult.Fail("Unauthorized.");
            }
        }

        var claims = new List<Claim>
        {
            new(AppClaimTypes.ClientId, client.Id.ToString()),
            new(AppClaimTypes.ClientSecret, client.ClientType.ToString())
        };
        
        var identity = new ClaimsIdentity(claims, ClientSecretBasicAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, ClientSecretBasicAuthenticationDefaults.AuthenticationScheme);
        
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