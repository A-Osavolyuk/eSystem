using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.Claims;
using eSecurity.Server.Security.Identity.Claims.Factories;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Authentication.OpenIdConnect.Token;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Token.Strategies;

public sealed class ClientCredentialsContext : TokenContext
{
    public string? ClientSecret { get; set; }
    public string? Scope { get; set; }
}

public sealed class ClientCredentialsStrategy(
    IClientManager clientManager,
    IClaimFactoryProvider claimFactoryProvider,
    ITokenFactoryProvider tokenFactoryProvider,
    IOptions<TokenOptions> options) : ITokenStrategy
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly IClaimFactoryProvider _claimFactoryProvider = claimFactoryProvider;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly TokenOptions _options = options.Value;

    public async ValueTask<Result> ExecuteAsync(TokenContext context, 
        CancellationToken cancellationToken = default)
    {
        if (context is not ClientCredentialsContext clientCredentialsContext)
            throw new NotSupportedException("Payload type must be 'ClientCredentialsContext'");
        
        if (string.IsNullOrEmpty(clientCredentialsContext.ClientSecret))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "client_secret is required"
            });
        }
        
        if (string.IsNullOrEmpty(clientCredentialsContext.Scope))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "scope is required"
            });
        }

        var client = await _clientManager.FindByIdAsync(clientCredentialsContext.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.Unauthorized(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidClient,
                Description = "Client was not found."
            });
        }

        if (!client.HasGrantType(clientCredentialsContext.GrantType))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.UnsupportedGrantType,
                Description = $"'{clientCredentialsContext.GrantType}' grant is not supported by client."
            });
        }

        var grantedScopes = client.AllowedScopes.Select(x => x.Scope.Name);
        var requestScopes = clientCredentialsContext.Scope!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var allowedScopes = grantedScopes.Intersect(requestScopes).ToList();
        
        if (allowedScopes.Count == 0)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidScope,
                Description = "None of the requested scopes are allowed for this client."
            });
        }

        var response = new TokenResponse()
        {
            ExpiresIn = (int)_options.AccessTokenLifetime.TotalSeconds,
            TokenType = ResponseTokenTypes.Bearer
        };
        
        if (client.AccessTokenType == AccessTokenType.Jwt)
        {
            var claimsFactory = _claimFactoryProvider.GetClaimFactory<AccessTokenClaimsContext, ClientEntity>();
            var claimsContext = new AccessTokenClaimsContext()
            {
                Aud = client.Audience,
                Scopes = allowedScopes
            };
            
            var claims = await claimsFactory.GetClaimsAsync(client, claimsContext, cancellationToken);
            var jwtTokenContext = new JwtTokenContext
            {
                Claims = claims,
                Type = JwtTokenTypes.AccessToken
            };
            
            var tokenFactory = _tokenFactoryProvider.GetFactory<JwtTokenContext, string>();
            var token = await tokenFactory.CreateTokenAsync(jwtTokenContext, cancellationToken);
            response.AccessToken = token;
        }
        else
        {
            //TODO: Implement Opaque Token generation
        }
        
        return Results.Ok(response);
    }
}