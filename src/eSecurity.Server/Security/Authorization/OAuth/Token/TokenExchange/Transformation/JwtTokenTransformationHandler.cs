using System.Security.Claims;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Constants;
using eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Delegation;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authorization.OAuth.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token.TokenExchange;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Transformation;

public sealed class JwtTokenTransformationHandler(
    ITokenClaimsExtractor claimsExtractor,
    IClientManager clientManager,
    ITokenFactoryProvider tokenFactoryProvider,
    IOptions<TokenOptions> options) : ITokenTransformationHandler
{
    private readonly ITokenClaimsExtractor _claimsExtractor = claimsExtractor;
    private readonly IClientManager _clientManager = clientManager;
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly TokenOptions _options = options.Value;

    public async ValueTask<Result> HandleAsync(TokenExchangeFlowContext context,
        CancellationToken cancellationToken = default)
    {
        var extractionResult = await _claimsExtractor.ExtractAsync(context.SubjectToken, cancellationToken);
        if (!extractionResult.IsSucceeded || extractionResult.Value is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Subject token is invalid"
            });
        }

        var claims = extractionResult.Value.ToList();
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

            claims.Add(new Claim(AppClaimTypes.Aud, context.Audience));
        }

        var scopes = context.Scope.Split(' ').ToList();
        var subjectScopes = claims
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

        if (claims.Any(x => x.Type == AppClaimTypes.Scope))
        {
            var scopeClaim = claims.First(x => x.Type == AppClaimTypes.Scope);
            claims.Remove(scopeClaim);
        }

        claims.Add(new Claim(AppClaimTypes.Scope, context.Scope));

        var tokenFactory = _tokenFactoryProvider.GetFactory<JwtTokenContext, string>();
        var tokenContext = new JwtTokenContext { Claims = claims, Type = JwtTokenTypes.AccessToken };
        var accessToken = await tokenFactory.CreateTokenAsync(tokenContext, cancellationToken);

        var response = new TokenExchangeResponse
        {
            ExpiresIn = (int)_options.DefaultAccessTokenLifetime.TotalSeconds,
            TokenType = ResponseTokenTypes.Bearer,
            IssuedTokenType = TokenTypes.Full.AccessToken,
            Scope = context.Scope,
            Audience = context.Audience,
            AccessToken = accessToken,
            IssuedAt = long.Parse(claims.First(x => x.Type == AppClaimTypes.Iat).Value)
        };

        return Results.Ok(response);
    }
}