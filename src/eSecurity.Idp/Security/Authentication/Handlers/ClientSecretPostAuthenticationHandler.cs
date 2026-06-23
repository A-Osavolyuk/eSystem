using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using eSecurity.Idp.Security.Authentication.Client;
using eSystem.Core.Enums;
using eSystem.Core.Http.Constants;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Security.Identity.Claims;
using eSystem.Core.Server.Security.Authentication.Schemes;
using Microsoft.AspNetCore.Authentication;

namespace eSecurity.Idp.Security.Authentication.Handlers;

public sealed class ClientSecretPostAuthenticationSchemeOptions : AuthenticationSchemeOptions {}

public class ClientSecretPostAuthenticationHandler(
    IOptionsMonitor<ClientSecretPostAuthenticationSchemeOptions> options, 
    ILoggerFactory logger,
    IClientQueryService clientQueryService,
    UrlEncoder encoder) : AuthenticationHandler<ClientSecretPostAuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly IClientQueryService _clientQueryService = clientQueryService;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.HasFormContentType)
        {
            Context.Items["error"] = ErrorCode.InvalidClient;
            return AuthenticateResult.Fail("Unauthorized.");
        }
        
        var body = await Request.ReadFormAsync();
        if (!body.TryGetValue("client_id", out var clientId))
        {
            Context.Items["error"] = ErrorCode.InvalidClient;
            return AuthenticateResult.Fail("Unauthorized.");
        }
        
        var client = await _clientQueryService.GetByIdAsync(clientId.ToString());
        if (client is null)
        {
            Context.Items["error"] = ErrorCode.InvalidClient;
            return AuthenticateResult.Fail("Unauthorized.");
        }

        if (client is { ClientType: ClientType.Confidential, RequireClientSecret: true })
        {
            if (!body.TryGetValue("client_secret", out var secret) ||
                !CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(client.Secret!), 
                    Encoding.UTF8.GetBytes(secret.ToString())))
            {
                Context.Items["error"] = ErrorCode.InvalidClient;
                return AuthenticateResult.Fail("Unauthorized.");
            }
        }

        var claims = new List<Claim>
        {
            new(AppClaimTypes.ClientId, client.Id.ToString()),
            new(AppClaimTypes.ClientSecret, client.ClientType.ToString())
        };

        var identity = new ClaimsIdentity(claims, AuthenticationSchemes.ClientSecretPost);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationSchemes.ClientSecretPost);

        return AuthenticateResult.Success(ticket);
    }
    
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var errorString = Context.Items["error"]?.ToString() ?? string.Empty;
        var error = EnumHelper.ParseFromString<ErrorCode>(errorString);
        if (error is null || error.Value is ErrorCode.ServerError)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            Response.ContentType = ContentTypes.Application.Json;
        
            await Response.WriteAsJsonAsync(new Error
            {
                Code = ErrorCode.ServerError, 
                Description = "Server error."
            });
        }
        else
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            Response.ContentType = ContentTypes.Application.Json;
        
            await Response.WriteAsJsonAsync(new Error
            {
                Code = error.Value, 
                Description = "Invalid client."
            });
        }
    }
}