using eSecurity.Server.Security.Authorization.OAuth.Protocol;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token;
using eSystem.Core.Security.Authorization.OAuth.Token.RefreshToken;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.RefreshToken;

public class RefreshTokenStrategy(
    ITokenManager tokenManager,
    IHasherProvider hasherProvider,
    IRefreshTokenFlowResolver resolver) : ITokenStrategy
{
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IHasherProvider _hasherProvider = hasherProvider;
    private readonly IRefreshTokenFlowResolver _resolver = resolver;

    public async ValueTask<Result> ExecuteAsync(TokenRequest tokenRequest,
        CancellationToken cancellationToken = default)
    {
        if (tokenRequest is not RefreshTokenRequest request)
            throw new NotSupportedException("Payload type must be 'RefreshTokenRequest'.");

        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            return Results.NotFound(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid refresh token."
            });
        }
        
        var hasher = _hasherProvider.GetHasher(HashAlgorithm.Sha512);
        var incomingHash = hasher.Hash(request.RefreshToken!);
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
            ClientId = request.ClientId,
            GrantType = request.GrantType,
            RefreshToken = request.RefreshToken
        };

        return await flow.ExecuteAsync(refreshToken, refreshTokenContext, cancellationToken);
    }
}