using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authorization.Protocol;
using eSecurity.Server.Security.Authorization.Token.RefreshToken;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.Claims;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Server.Security.Authorization.Token.Strategies;

public sealed class RefreshTokenContext : TokenContext
{
    public required string RefreshToken { get; set; }
}

public class RefreshTokenStrategy(
    ITokenFactoryProvider tokenFactoryProvider,
    IClientManager clientManager,
    ITokenManager tokenManager,
    IUserManager userManager,
    IHasherProvider hasherProvider,
    ISessionManager sessionManager,
    IClaimFactoryProvider claimFactoryProvider,
    IOptions<TokenOptions> options,
    IRefreshTokenFlowResolver resolver) : ITokenStrategy
{
    private readonly ITokenFactoryProvider _tokenFactoryProvider = tokenFactoryProvider;
    private readonly IClientManager _clientManager = clientManager;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IUserManager _userManager = userManager;
    private readonly IHasherProvider _hasherProvider = hasherProvider;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IClaimFactoryProvider _claimFactoryProvider = claimFactoryProvider;
    private readonly IRefreshTokenFlowResolver _resolver = resolver;
    private readonly TokenOptions _options = options.Value;

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

        var protocol = refreshToken.Scopes.Any(x => x.Scope == ScopeTypes.OpenId)
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