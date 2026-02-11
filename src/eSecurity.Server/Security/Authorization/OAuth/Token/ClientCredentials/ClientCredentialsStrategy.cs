using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.Claims;
using eSecurity.Server.Security.Identity.Claims.Factories;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token;
using eSystem.Core.Security.Authorization.OAuth.Token.ClientCredentials;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.ClientCredentials;

public sealed class ClientCredentialsStrategy(
    IClientManager clientManager,
    IClaimFactoryProvider claimFactoryProvider,
    ITokenFactoryProvider tokenFactoryProvider,
    IHasherProvider hasherProvider,
    ITokenManager tokenManager,
    IOptions<TokenConfigurations> options) : ITokenStrategy
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly IClaimFactoryProvider _claimFactoryProvider = claimFactoryProvider;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly IHasherProvider _hasherProvider = hasherProvider;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly TokenConfigurations _tokenConfigurations = options.Value;

    public async ValueTask<Result> ExecuteAsync(TokenRequest tokenRequest, 
        CancellationToken cancellationToken = default)
    {
        if (tokenRequest is not ClientCredentialsRequest request)
            throw new NotSupportedException("Payload type must be 'ClientCredentialsRequest'");
        
        if (string.IsNullOrEmpty(request.ClientSecret))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "client_secret is required"
            });
        }
        
        if (string.IsNullOrEmpty(request.Scope))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "scope is required"
            });
        }

        var client = await _clientManager.FindByIdAsync(request.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.Unauthorized(new Error
            {
                Code = ErrorTypes.OAuth.InvalidClient,
                Description = "Client was not found."
            });
        }

        if (!client.HasGrantType(request.GrantType))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.UnsupportedGrantType,
                Description = $"'{request.GrantType}' grant is not supported by client."
            });
        }

        var grantedScopes = client.AllowedScopes.Select(x => x.Scope.Value);
        var requestScopes = request.Scope!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var allowedScopes = grantedScopes.Intersect(requestScopes).ToList();
        
        if (allowedScopes.Count == 0)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidScope,
                Description = "None of the requested scopes are allowed for this client."
            });
        }

        var response = new ClientCredentialsResponse
        {
            ExpiresIn = (int)_tokenConfigurations.DefaultAccessTokenLifetime.TotalSeconds,
            TokenType = ResponseTokenTypes.Bearer
        };
        
        if (client.AccessTokenType == AccessTokenType.Jwt)
        {
            var lifetime = client.AccessTokenLifetime ?? _tokenConfigurations.DefaultAccessTokenLifetime;
            var claimsFactory = _claimFactoryProvider.GetClaimFactory<AccessTokenClaimsContext, ClientEntity>();
            var claimsContext = new AccessTokenClaimsContext
            {
                Exp = DateTimeOffset.UtcNow.Add(lifetime),
                Aud = client.Audiences.Select(x => x.Audience),
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
            var lifetime = client.AccessTokenLifetime ?? _tokenConfigurations.DefaultAccessTokenLifetime;
            var tokenContext = new OpaqueTokenContext
            {
                TokenLength = _tokenConfigurations.OpaqueTokenLength,
                TokenType = OpaqueTokenType.AccessToken,
                ClientId = client.Id,
                Audiences = client.Audiences.Select(x => x.Audience).ToList(),
                Scopes = client.AllowedScopes.Select(x => x.Scope.Value).ToList(),
                ExpiredAt = DateTimeOffset.UtcNow.Add(lifetime),
                Subject = client.Id.ToString(),
            };
            
            var tokenFactory = _tokenFactoryProvider.GetFactory<OpaqueTokenContext, string>();
            response.AccessToken = await tokenFactory.CreateTokenAsync(tokenContext, cancellationToken);
        }
        
        return Results.Ok(response);
    }
}