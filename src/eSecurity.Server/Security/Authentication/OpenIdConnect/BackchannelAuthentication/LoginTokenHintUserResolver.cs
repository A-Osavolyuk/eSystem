using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authorization.OAuth.Token;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
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

    public async Task<TypedResult<UserEntity>> ResolveAsync(BackchannelAuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.LoginTokenHint))
        {
            return TypedResult<UserEntity>.Fail(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "login_token_hint is invalid"
            });
        }
        
        var hash = _hasher.Hash(request.LoginTokenHint);
        var token = await _tokenManager.FindByHashAsync(hash, cancellationToken);
        if (token?.TokenType is not OpaqueTokenType.LoginToken)
        {
            return TypedResult<UserEntity>.Fail(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "login_token_hint is invalid"
            });
        }

        if (token.ExpiredAt < DateTimeOffset.UtcNow)
        {
            return TypedResult<UserEntity>.Fail(new Error()
            {
                Code = ErrorTypes.OAuth.ExpiredLoginTokenHint,
                Description = "login_token_hint is expired"
            });
        }

        var user = await _userManager.FindByIdAsync(Guid.Parse(token.Subject), cancellationToken);
        if (user is null)
        {
            return TypedResult<UserEntity>.Fail(new Error()
            {
                Code = ErrorTypes.OAuth.UnknownUserId,
                Description = "Unknown user"
            });
        }
        
        return TypedResult<UserEntity>.Success(user);
    }
}