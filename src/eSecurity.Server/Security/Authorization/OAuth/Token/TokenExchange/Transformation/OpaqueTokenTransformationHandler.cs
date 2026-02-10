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
    private readonly TokenOptions _configurations = options.Value;
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

        var tokenContext = new OpaqueTokenContext
        {
            TokenLength = _configurations.OpaqueTokenLength,
            TokenType = OpaqueTokenType.AccessToken,
            ClientId = client.Id,
            Audiences = client.Audiences.Select(x => x.Audience).ToList(),
            Scopes = client.AllowedScopes.Select(x => x.Scope.Value).ToList(),
            ExpiredAt = DateTimeOffset.UtcNow.Add(_configurations.DefaultAccessTokenLifetime),
            IssuedAt = DateTimeOffset.UtcNow,
            Subject = token.Subject,
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

            if (tokenContext.Audiences.All(x => x != context.Audience))
                tokenContext.Audiences.Add(context.Audience);
        }

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

        tokenContext.Scopes = scopes;

        var tokenFactory = _tokenFactoryProvider.GetFactory<OpaqueTokenContext, string>();
        var opaqueToken = await tokenFactory.CreateTokenAsync(tokenContext, cancellationToken);

        return Results.Ok(new TokenExchangeResponse
        {
            ExpiresIn = (int)_configurations.DefaultAccessTokenLifetime.TotalSeconds,
            TokenType = ResponseTokenTypes.Bearer,
            IssuedTokenType = TokenTypes.Full.AccessToken,
            Scope = context.Scope,
            Audience = JsonSerializer.Serialize(tokenContext.Audiences),
            AccessToken = opaqueToken,
            IssuedAt = tokenContext.IssuedAt!.Value.ToUnixTimeSeconds(),
        });
    }
}