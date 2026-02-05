using eSecurity.Server.Security.Authorization.OAuth.Protocol;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.RefreshToken;

public sealed class RefreshTokenContext : TokenContext
{
    public required string RefreshToken { get; set; }
}

public class RefreshTokenStrategy(
    ITokenManager tokenManager,
    IHasherProvider hasherProvider,
    IRefreshTokenFlowResolver resolver) : ITokenStrategy
{
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IHasherProvider _hasherProvider = hasherProvider;
    private readonly IRefreshTokenFlowResolver _resolver = resolver;

    public async ValueTask<Result> ExecuteAsync(TokenContext context,
        CancellationToken cancellationToken = default)
    {
        if (context is not RefreshTokenContext refreshPayload)
            throw new NotSupportedException("Payload type must be 'RefreshTokenPayload'.");

        var hasher = _hasherProvider.GetHasher(HashAlgorithm.Sha512);
        var incomingHash = hasher.Hash(refreshPayload.RefreshToken!);
        var refreshToken = await _tokenManager.FindByHashAsync(incomingHash, cancellationToken);
        if (refreshToken is null || !refreshToken.IsValid)
        {
            return Results.NotFound(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid refresh token."
            });
        }

        var protocol = refreshToken.Scopes.Any(x => x.ClientScope.Scope.Value == ScopeTypes.OpenId)
            ? AuthorizationProtocol.OpenIdConnect
            : AuthorizationProtocol.OAuth;

        var flow = _resolver.Resolve(protocol);
        var refreshTokenContext = new RefreshTokenFlowContext()
        {
            ClientId = refreshPayload.ClientId,
            GrantType = refreshPayload.GrantType,
            RefreshToken = refreshPayload.RefreshToken
        };

        return await flow.ExecuteAsync(refreshToken, refreshTokenContext, cancellationToken);
    }
}