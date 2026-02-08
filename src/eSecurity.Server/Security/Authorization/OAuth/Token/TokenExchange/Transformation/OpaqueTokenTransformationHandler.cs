using System.Text.Json;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token.TokenExchange;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Transformation;

public sealed class OpaqueTokenTransformationHandler(
    IHasherProvider hasherProvider,
    ITokenManager tokenManager,
    IClientManager clientManager,
    IOptions<TokenOptions> options,
    ITokenFactoryProvider tokenFactoryProvider) : ITokenTransformationHandler
{
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Sha512);
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IClientManager _clientManager = clientManager;
    private readonly TokenOptions _options = options.Value;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;

    public async ValueTask<Result> HandleAsync(TokenExchangeFlowContext context,
        CancellationToken cancellationToken = default)
    {
        if (!await _tokenManager.IsOpaqueAsync(context.SubjectToken, cancellationToken))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Subject token is invalid."
            });
        }

        var hash = _hasher.Hash(context.SubjectToken);
        var token = await _tokenManager.FindByHashAsync(hash, cancellationToken);
        if (token is null || !token.IsValid)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Subject token is invalid."
            });
        }

        var client = await _clientManager.FindByIdAsync(context.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Subject token is invalid."
            });
        }

        var tokenContext = new OpaqueTokenContext { Length = _options.OpaqueTokenLength };
        var tokenFactory = _tokenFactoryProvider.GetFactory<OpaqueTokenContext, string>();
        var opaqueToken = await tokenFactory.CreateTokenAsync(tokenContext, cancellationToken);
        var transformedToken = new OpaqueTokenEntity
        {
            Id = Guid.CreateVersion7(),
            ClientId = client.Id,
            TokenHash = _hasher.Hash(opaqueToken),
            Subject = token.Subject,
            TokenType = OpaqueTokenType.AccessToken,
            IssuedAt = DateTimeOffset.UtcNow,
            ExpiredAt = DateTimeOffset.UtcNow.Add(_options.AccessTokenLifetime),
            Audiences = token.Audiences,
            Scopes = token.Scopes
        };
        
        if (!string.IsNullOrEmpty(context.Audience))
        {
            if (!client.IsValidAudience(context.Audience))
            {
                return Results.BadRequest(new Error()
                {
                    Code = ErrorTypes.OAuth.InvalidTarget,
                    Description = "The requested audience is not an allowed audience for this client."
                });
            }

            if (transformedToken.Audiences.All(x => x.Audience.Audience != context.Audience))
            {
                var audience = client.Audiences.First(x => x.Audience == context.Audience);
                transformedToken.Audiences.Add(new OpaqueTokenAudienceEntity()
                {
                    Id = Guid.CreateVersion7(),
                    TokenId = transformedToken.Id,
                    AudienceId = audience.Id
                });
            }
        }
        
        if (!string.IsNullOrEmpty(context.Scope))
        {
            var scopes = context.Scope.Split(' ').ToList();
            var subjectScopes = token.Scopes
                .Select(x => x.ClientScope.Scope.Value)
                .ToHashSet();

            if (!scopes.All(s => subjectScopes.Contains(s)))
            {
                return Results.BadRequest(new Error
                {
                    Code = ErrorTypes.OAuth.InvalidScope,
                    Description = "Requested scopes exceed the subject token scopes."
                });
            }

            transformedToken.Scopes = client.AllowedScopes.Select(x => new OpaqueTokenScopeEntity
            {
                Id = Guid.CreateVersion7(),
                TokenId = transformedToken.Id,
                ScopeId = x.Id
            }).ToList();
        }

        var aud = JsonSerializer.Serialize(transformedToken.Audiences
            .Select(x => x.Audience.Audience)
            .ToArray()
        );
        
        var response = new TokenExchangeResponse
        {
            ExpiresIn = (int)_options.AccessTokenLifetime.TotalSeconds,
            TokenType = ResponseTokenTypes.Bearer,
            IssuedTokenType = TokenTypes.Full.AccessToken,
            Scope = context.Scope,
            Audience = aud,
            AccessToken = opaqueToken,
            IssuedAt = transformedToken.IssuedAt.ToUnixTimeSeconds(),
        };
        
        var result = await _tokenManager.CreateAsync(transformedToken, cancellationToken);
        return result.Succeeded ? Results.Ok(response) : result;
    }
}