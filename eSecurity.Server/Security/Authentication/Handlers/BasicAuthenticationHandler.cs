using System.Security.Claims;
using System.Text.Encodings.Web;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSystem.Core.Common.Http.Constants;
using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Server.Security.Authentication.Handlers;

public class BasicAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
}

public static class BasicAuthenticationDefaults
{
    public const string AuthenticationScheme = "Basic";
}

public class BasicAuthenticationHandler(
    IOptionsMonitor<BasicAuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    IClientManager clientManager,
    UrlEncoder encoder) : AuthenticationHandler<BasicAuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly IClientManager _clientManager = clientManager;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(header) && header.StartsWith($"{AuthenticationTypes.Basic} "))
        {
            var base64 = header[$"{AuthenticationTypes.Basic} ".Length..];
            var bytes = Convert.FromBase64String(base64);
            var value = Encoding.ASCII.GetString(bytes);
            if (!value.Contains(':'))
            {
                Context.Items["error"] = Errors.OAuth.InvalidClient;
                return AuthenticateResult.Fail("Unauthorized.");
            }

            var valueParts = value.Split(':', 2);
            if (valueParts.Length != 2)
            {
                Context.Items["error"] = Errors.OAuth.InvalidClient;
                return AuthenticateResult.Fail("Unauthorized.");
            }


            var clientId = valueParts.First();
            var client = await _clientManager.FindByIdAsync(clientId);
            if (client is null)
            {
                Context.Items["error"] = Errors.OAuth.InvalidClient;
                return AuthenticateResult.Fail("Unauthorized.");
            }

            var clientSecret = valueParts.Last();
            if (client.Type == ClientType.Confidential && !client.Secret.Equals(clientSecret))
            {
                Context.Items["error"] = Errors.OAuth.InvalidClient;
                return AuthenticateResult.Fail("Unauthorized.");
            }

            var ticket = CreateTicket(client);
            return AuthenticateResult.Success(ticket);
        }
        else
        {
            var body = await Request.ReadFormAsync();

            if (!body.TryGetValue("client_id", out var clientId))
            {
                Context.Items["error"] = Errors.OAuth.InvalidClient;
                return AuthenticateResult.Fail("Unauthorized.");
            }

            var client = await _clientManager.FindByIdAsync(clientId.ToString());
            if (client is null)
            {
                Context.Items["error"] = Errors.OAuth.InvalidClient;
                return AuthenticateResult.Fail("Unauthorized.");
            }

            if (client.Type == ClientType.Confidential)
            {
                if (!body.TryGetValue("client_secret", out var secret) || !client.Secret.Equals(secret))
                {
                    Context.Items["error"] = Errors.OAuth.InvalidClient;
                    return AuthenticateResult.Fail("Unauthorized.");
                }
            }

            var ticket = CreateTicket(client);
            return AuthenticateResult.Success(ticket);
        }
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.ContentType = ContentTypes.Application.Json;

        var error = new Error()
        {
            Code = Context.Items["error"]?.ToString() ?? Errors.OAuth.InvalidClient, 
            Description = "Invalid client."
        };
        
        await Response.WriteAsJsonAsync(error);
    }

    private AuthenticationTicket CreateTicket(ClientEntity client)
    {
        var claims = new List<Claim>
        {
            new("client_id", client.Id.ToString()),
            new("client_type", client.Type.ToString())
        };
        var identity = new ClaimsIdentity(claims, BasicAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        return new AuthenticationTicket(principal, BasicAuthenticationDefaults.AuthenticationScheme);
    }
}