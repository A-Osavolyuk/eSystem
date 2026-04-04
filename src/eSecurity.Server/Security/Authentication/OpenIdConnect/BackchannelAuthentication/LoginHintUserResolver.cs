using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

public sealed class LoginHintUserResolver(IUserManager userManager) : IUserResolver
{
    private readonly IUserManager _userManager = userManager;
    
    public async Task<TypedResult<UserEntity>> ResolveAsync(BackchannelAuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.LoginHint))
        {
            return TypedResult<UserEntity>.Fail(new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "login_hint is invalid"
            });
        }
        
        var user = await _userManager.FindByLoginAsync(request.LoginHint, cancellationToken);
        if (user is null)
        {
            return TypedResult<UserEntity>.Fail(new Error()
            {
                Code = ErrorCode.UnknownUserId,
                Description = "Unknown user"
            });
        }
        
        return TypedResult<UserEntity>.Success(user);
    }
}