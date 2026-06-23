using System.Text.Json;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Cryptography.Hashing;
using eSecurity.Idp.Security.Cryptography.Tokens;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Server.Security.Authorization.OAuth.Token.TokenExchange;

namespace eSecurity.Idp.Security.Authorization.Token.TokenExchange.Delegation;

public sealed class OpaqueTokenDelegationHandler(
    IHasherProvider hasherProvider,
    ITokenManager tokenManager,
    IOptions<TokenConfigurations> options,
    IClientQueryService clientQueryService,
    ITokenBuilderProvider tokenBuilderProvider) : ITokenDelegationHandler
{
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IClientQueryService _clientQueryService = clientQueryService;
    private readonly TokenConfigurations _configurations = options.Value;
    private readonly ITokenBuilderProvider _tokenBuilderProvider = tokenBuilderProvider;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Sha512);

    public async ValueTask<Result> HandleAsync(TokenExchangeFlowContext context,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(context.ActorToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "actor_token is required"
            });
        }

        if (context.ActorTokenType is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "actor_token_type is required"
            });
        }

        if (context.ActorTokenType != TokenType.AccessToken)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = $"{TokenType.AccessToken} is the only allowed actor_token_type value"
            });
        }

        if (!await _tokenManager.IsOpaqueAsync(context.SubjectToken, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Subject token is invalid."
            });
        }

        if (!await _tokenManager.IsOpaqueAsync(context.ActorToken, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Actor token is invalid."
            });
        }

        var subjectTokenHash = _hasher.Hash(context.SubjectToken);
        var subjectToken = await _tokenManager.FindByHashAsync(subjectTokenHash, cancellationToken);
        if (subjectToken is null || !subjectToken.IsValid)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Subject token is invalid."
            });
        }

        var actorTokenHash = _hasher.Hash(context.ActorToken);
        var actorToken = await _tokenManager.FindByHashAsync(actorTokenHash, cancellationToken);
        if (actorToken is null || !actorToken.IsValid || actorToken.TokenType != OpaqueTokenType.AccessToken)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Actor token is invalid."
            });
        }

        if (subjectToken.IsDelegated || actorToken.IsDelegated)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Delegation chaining is not allowed."
            });
        }
        
        var client = await _clientQueryService.GetByIdAsync(context.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidGrant,
                Description = "Subject token is invalid."
            });
        }

        var clientScopes = await _clientQueryService.GetAllowedScopesAsync(client, cancellationToken);
        var clientAudiences = await _clientQueryService.GetSupportedAudiencesAsync(client, cancellationToken);
        
        var tokenContext = new OpaqueTokenBuildContext
        {
            TokenLength = _configurations.OpaqueTokenLength,
            TokenType = OpaqueTokenType.AccessToken,
            ClientId = client.Id,
            Audiences = clientAudiences.Select(x => x.Audience).ToList(),
            Scopes = clientScopes.Select(x => x.Scope.Value).ToList(),
            ExpiredAt = DateTimeOffset.UtcNow.Add(_configurations.DefaultAccessTokenLifetime),
            IssuedAt = DateTimeOffset.UtcNow,
            Subject = subjectToken.Subject,
            ActorId = actorToken.Id
        };
        
        if (!string.IsNullOrEmpty(context.Audience))
        {
            if (clientAudiences.All(x => x.Audience != context.Audience))
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidTarget,
                    Description = "The requested audience is not an allowed audience for this client."
                });
            }

            if (tokenContext.Audiences.All(x => x != context.Audience))
                tokenContext.Audiences.Add(context.Audience);
        }

        var scopes = context.Scope.Split(' ').ToList();
        var subjectScopes = subjectToken.Scopes
            .Select(x => x.ClientScope.Scope.Value)
            .ToHashSet();

        if (!scopes.All(s => subjectScopes.Contains(s)))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidScope,
                Description = "Requested scopes exceed the subject token scopes."
            });
        }

        tokenContext.Scopes = scopes;
        
        var tokenFactory = _tokenBuilderProvider.GetFactory<OpaqueTokenBuildContext, string>();
        var opaqueToken = await tokenFactory.BuildAsync(tokenContext, cancellationToken);

        return Results.Success(SuccessCodes.Ok, new TokenExchangeResponse
        {
            ExpiresIn = (int)_configurations.DefaultAccessTokenLifetime.TotalSeconds,
            TokenType = ResponseTokenType.Bearer,
            IssuedTokenType = TokenType.AccessToken,
            Scope = context.Scope,
            Audience = JsonSerializer.Serialize(tokenContext.Audiences),
            AccessToken = opaqueToken,
            IssuedAt = tokenContext.IssuedAt!.Value.ToUnixTimeSeconds(),
        });
    }
}