using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

namespace eSecurity.Idp.Security.Authentication.BackchannelAuthentication;

public sealed class LoginHintUserResolver(IUserQueryService userQueryService) : IUserResolver
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    
    public async Task<TypedResult<UserEntity>> ResolveAsync(BackchannelAuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.LoginHint))
        {
            return TypedResult<UserEntity>.Fail(new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "login_hint is invalid"
            });
        }
        
        var user = await _userQueryService.GetByLoginAsync(request.LoginHint, cancellationToken);
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