using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authorization.OAuth.Token;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

public sealed class LoginTokenHintUserResolver(
    ITokenManager tokenManager,
    IHasherProvider hasherProvider,
    IUserManager userManager) : IUserResolver
{
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IUserManager _userManager = userManager;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Sha512);

    public async Task<UserEntity?> ResolveAsync(BackchannelAuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.LoginTokenHint))
            return null;
        
        var hash = _hasher.Hash(request.LoginTokenHint);
        var token = await _tokenManager.FindByHashAsync(hash, cancellationToken);
        if (token?.TokenType is not OpaqueTokenType.LoginToken || token.ExpiredAt < DateTimeOffset.UtcNow)
            return null;

        return await _userManager.FindByIdAsync(Guid.Parse(token.Subject), cancellationToken);
    }
}