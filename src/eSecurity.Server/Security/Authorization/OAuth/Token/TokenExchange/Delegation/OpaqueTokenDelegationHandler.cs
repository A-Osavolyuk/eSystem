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

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Delegation;

public sealed class OpaqueTokenDelegationHandler(
    IHasherProvider hasherProvider,
    ITokenManager tokenManager,
    IClientManager clientManager,
    IOptions<TokenOptions> options,
    ITokenFactoryProvider tokenFactoryProvider) : ITokenDelegationHandler
{
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IClientManager _clientManager = clientManager;
    private readonly TokenOptions _options = options.Value;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Sha512);

    public async ValueTask<Result> HandleAsync(TokenExchangeFlowContext context,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(context.ActorToken))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "actor_token is required"
            });
        }

        if (string.IsNullOrEmpty(context.ActorTokenType))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "actor_token_type is required"
            });
        }

        if (context.ActorTokenType != TokenTypes.Full.AccessToken)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = $"{TokenTypes.Full.AccessToken} is the only allowed actor_token_type value"
            });
        }

        if (!await _tokenManager.IsOpaqueAsync(context.SubjectToken, cancellationToken))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Subject token is invalid."
            });
        }

        if (!await _tokenManager.IsOpaqueAsync(context.ActorToken, cancellationToken))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Actor token is invalid."
            });
        }

        var subjectTokenHash = _hasher.Hash(context.SubjectToken);
        var subjectToken = await _tokenManager.FindByHashAsync(subjectTokenHash, cancellationToken);
        if (subjectToken is null || !subjectToken.IsValid)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Subject token is invalid."
            });
        }

        var actorTokenHash = _hasher.Hash(context.ActorToken);
        var actorToken = await _tokenManager.FindByHashAsync(actorTokenHash, cancellationToken);
        if (actorToken is null || !actorToken.IsValid || actorToken.TokenType != OpaqueTokenType.AccessToken)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Actor token is invalid."
            });
        }

        if (subjectToken.IsDelegated || actorToken.IsDelegated)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Delegation chaining is not allowed."
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
        var delegatedToken = new OpaqueTokenEntity
        {
            Id = Guid.CreateVersion7(),
            ClientId = client.Id,
            TokenHash = _hasher.Hash(opaqueToken),
            Subject = subjectToken.Subject,
            TokenType = OpaqueTokenType.AccessToken,
            IssuedAt = DateTimeOffset.UtcNow,
            ExpiredAt = DateTimeOffset.UtcNow.Add(_options.AccessTokenLifetime),
            Audiences = subjectToken.Audiences,
            Scopes = subjectToken.Scopes,
            ActorId = actorToken.Id
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

            if (delegatedToken.Audiences.All(x => x.Audience.Audience != context.Audience))
            {
                var audience = client.Audiences.First(x => x.Audience == context.Audience);
                delegatedToken.Audiences.Add(new OpaqueTokenAudienceEntity()
                {
                    Id = Guid.CreateVersion7(),
                    TokenId = delegatedToken.Id,
                    AudienceId = audience.Id
                });
            }
        }

        var scopes = context.Scope.Split(' ').ToList();
        var subjectScopes = subjectToken.Scopes
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

        delegatedToken.Scopes = subjectToken.Scopes
            .Where(x => scopes.Contains(x.ClientScope.Scope.Value))
            .Select(x => new OpaqueTokenScopeEntity
            {
                Id = Guid.CreateVersion7(),
                TokenId = delegatedToken.Id,
                ScopeId = x.ScopeId
            })
            .ToList();

        var aud = JsonSerializer.Serialize(delegatedToken.Audiences
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
            IssuedAt = delegatedToken.IssuedAt.ToUnixTimeSeconds(),
        };

        var result = await _tokenManager.CreateAsync(delegatedToken, cancellationToken);
        return result.Succeeded ? Results.Ok(response) : result;
    }
}