using System.Security.Claims;
using System.Text.Json;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token.TokenExchange;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Delegation;

public sealed class JwtTokenDelegationHandler(
    ITokenClaimsExtractor claimsExtractor,
    IClientManager clientManager,
    ITokenBuilderProvider tokenBuilderProvider,
    IOptions<TokenConfigurations> options) : ITokenDelegationHandler
{
    private readonly ITokenClaimsExtractor _claimsExtractor = claimsExtractor;
    private readonly IClientManager _clientManager = clientManager;
    private readonly ITokenBuilderProvider _tokenBuilderProvider = tokenBuilderProvider;
    private readonly TokenConfigurations _configurations = options.Value;

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

        var actorTokenExtractionResult = await _claimsExtractor.ExtractAsync(context.ActorToken, cancellationToken);
        if (!actorTokenExtractionResult.IsSucceeded || actorTokenExtractionResult.Value is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Actor token is invalid"
            });
        }

        var subjectTokenExtractionResult = await _claimsExtractor.ExtractAsync(context.SubjectToken, cancellationToken);
        if (!subjectTokenExtractionResult.IsSucceeded || subjectTokenExtractionResult.Value is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Subject token is invalid"
            });
        }

        var actorTokenClaims = actorTokenExtractionResult.Value.ToList();
        var subjectTokenClaims = subjectTokenExtractionResult.Value.ToList();
        if (actorTokenClaims.Any(x => x.Type == AppClaimTypes.Delegated) ||
            subjectTokenClaims.Any(x => x.Type == AppClaimTypes.Delegated))
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Delegation chaining is not allowed"
            });
        }

        var client = await _clientManager.FindByIdAsync(context.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.Unauthorized(new Error()
            {
                Code = ErrorTypes.OAuth.UnauthorizedClient,
                Description = "Unauthorized client"
            });
        }

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

            subjectTokenClaims.Add(new Claim(AppClaimTypes.Aud, context.Audience));
        }

        var scopes = context.Scope.Split(' ').ToList();
        var subjectScopes = subjectTokenClaims
            .FirstOrDefault(x => x.Type == AppClaimTypes.Scope)?.Value
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToHashSet();

        if (subjectScopes is not null && !scopes.All(s => subjectScopes.Contains(s)))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidScope,
                Description = "Requested scopes exceed the subject token scopes."
            });
        }

        if (subjectTokenClaims.Any(x => x.Type == AppClaimTypes.Scope))
        {
            var scopeClaim = subjectTokenClaims.First(x => x.Type == AppClaimTypes.Scope);
            subjectTokenClaims.Remove(scopeClaim);
        }

        subjectTokenClaims.Add(new Claim(AppClaimTypes.Scope, context.Scope));

        var subClaim = actorTokenClaims.FirstOrDefault(x => x.Type == AppClaimTypes.Sub);
        var clientIdClaim = actorTokenClaims.First(x => x.Type == AppClaimTypes.ClientId);

        var actor = new ActorClaim()
        {
            Subject = subClaim?.Value ?? clientIdClaim.Value,
            ClientId = clientIdClaim.Value,
            Issuer = _configurations.Issuer,
            AuthenticationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        subjectTokenClaims.Add(new Claim(AppClaimTypes.Act, JsonSerializer.Serialize(actor)));
        subjectTokenClaims.Add(new Claim(AppClaimTypes.Delegated, "true"));

        var tokenFactory = _tokenBuilderProvider.GetFactory<JwtTokenBuildContext, string>();
        var tokenContext = new JwtTokenBuildContext { Claims = subjectTokenClaims, Type = JwtTokenTypes.AccessToken };
        var accessToken = await tokenFactory.BuildAsync(tokenContext, cancellationToken);

        var response = new TokenExchangeResponse
        {
            ExpiresIn = (int)_configurations.DefaultAccessTokenLifetime.TotalSeconds,
            TokenType = ResponseTokenTypes.Bearer,
            IssuedTokenType = TokenTypes.Full.AccessToken,
            Scope = context.Scope,
            Audience = context.Audience,
            AccessToken = accessToken,
            IssuedAt = long.Parse(subjectTokenClaims.First(x => x.Type == AppClaimTypes.Iat).Value)
        };

        return Results.Ok(response);
    }
}