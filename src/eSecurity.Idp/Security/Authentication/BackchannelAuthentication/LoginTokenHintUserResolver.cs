using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Features.Connect;
using eSecurity.Idp.Security.Authorization.Token;
using eSecurity.Idp.Security.Cryptography.Hashing;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.BackchannelAuthentication;

public sealed class LoginTokenHintUserResolver(
    ITokenManager tokenManager,
    IHasherProvider hasherProvider,
    IUserQueryService userQueryService) : IUserResolver
{
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Sha512);

    public async Task<TypedResult<UserEntity>> ResolveAsync(BackchannelAuthenticationCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(command.LoginTokenHint))
        {
            return TypedResult<UserEntity>.Fail(new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "login_token_hint is invalid"
            });
        }
        
        var hash = _hasher.Hash(command.LoginTokenHint);
        var token = await _tokenManager.FindByHashAsync(hash, cancellationToken);
        if (token?.TokenType is not OpaqueTokenType.LoginToken)
        {
            return TypedResult<UserEntity>.Fail(new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "login_token_hint is invalid"
            });
        }

        if (token.ExpiredAt < DateTimeOffset.UtcNow)
        {
            return TypedResult<UserEntity>.Fail(new Error
            {
                Code = ErrorCode.ExpiredLoginTokenHint,
                Description = "login_token_hint is expired"
            });
        }

        var user = await _userQueryService.GetByIdAsync(Guid.Parse(token.Subject), cancellationToken);
        if (user is null)
        {
            return TypedResult<UserEntity>.Fail(new Error
            {
                Code = ErrorCode.UnknownUserId,
                Description = "Unknown user"
            });
        }
        
        return TypedResult<UserEntity>.Success(user);
    }
}