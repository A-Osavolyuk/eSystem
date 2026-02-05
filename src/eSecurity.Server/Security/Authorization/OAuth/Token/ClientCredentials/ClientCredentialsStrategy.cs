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

namespace eSecurity.Server.Security.Authorization.OAuth.Token.ClientCredentials;

public sealed class ClientCredentialsContext : TokenContext
{
    public string? ClientSecret { get; set; }
    public string? Scope { get; set; }
}

public sealed class ClientCredentialsStrategy(
    IClientManager clientManager,
    IClaimFactoryProvider claimFactoryProvider,
    ITokenFactoryProvider tokenFactoryProvider,
    IHasherProvider hasherProvider,
    ITokenManager tokenManager,
    IOptions<TokenOptions> options) : ITokenStrategy
{
    private readonly IClientManager _clientManager = clientManager;
    private readonly IClaimFactoryProvider _claimFactoryProvider = claimFactoryProvider;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly IHasherProvider _hasherProvider = hasherProvider;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly TokenOptions _options = options.Value;

    public async ValueTask<Result> ExecuteAsync(TokenContext context, 
        CancellationToken cancellationToken = default)
    {
        if (context is not ClientCredentialsContext clientCredentialsContext)
            throw new NotSupportedException("Payload type must be 'ClientCredentialsContext'");
        
        if (string.IsNullOrEmpty(clientCredentialsContext.ClientSecret))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "client_secret is required"
            });
        }
        
        if (string.IsNullOrEmpty(clientCredentialsContext.Scope))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "scope is required"
            });
        }

        var client = await _clientManager.FindByIdAsync(clientCredentialsContext.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.Unauthorized(new Error
            {
                Code = ErrorTypes.OAuth.InvalidClient,
                Description = "Client was not found."
            });
        }

        if (!client.HasGrantType(clientCredentialsContext.GrantType))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.UnsupportedGrantType,
                Description = $"'{clientCredentialsContext.GrantType}' grant is not supported by client."
            });
        }

        var grantedScopes = client.AllowedScopes.Select(x => x.Scope.Value);
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

        var response = new TokenResponse
        {
            ExpiresIn = (int)_options.AccessTokenLifetime.TotalSeconds,
            TokenType = ResponseTokenTypes.Bearer
        };
        
        if (client.AccessTokenType == AccessTokenType.Jwt)
        {
            var claimsFactory = _claimFactoryProvider.GetClaimFactory<AccessTokenClaimsContext, ClientEntity>();
            var claimsContext = new AccessTokenClaimsContext
            {
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
            var tokenContext = new OpaqueTokenContext { Length = _options.RefreshTokenLength };
            var tokenFactory = _tokenFactoryProvider.GetFactory<OpaqueTokenContext, string>();
            var rawToken = await tokenFactory.CreateTokenAsync(tokenContext, cancellationToken);
            var hasher = _hasherProvider.GetHasher(HashAlgorithm.Sha512);
            var newRefreshToken = new OpaqueTokenEntity
            {
                Id = Guid.CreateVersion7(),
                ClientId = client.Id,
                Subject = client.Id.ToString(),
                TokenHash = hasher.Hash(rawToken),
                TokenType = OpaqueTokenType.AccessToken,
                ExpiredDate = DateTimeOffset.UtcNow.Add(_options.AccessTokenLifetime)
            };

            var scopes = client.AllowedScopes
                .Where(x => allowedScopes.Contains(x.Scope.Value));
            
            var createResult = await _tokenManager.CreateAsync(newRefreshToken, scopes, cancellationToken);
            if (!createResult.Succeeded) return createResult;
            
            response.AccessToken = rawToken;
        }
        
        return Results.Ok(response);
    }
}