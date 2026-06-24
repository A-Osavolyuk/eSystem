using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Features.Connect;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.BackchannelAuthentication;

public sealed class LoginHintUserResolver(IUserQueryService userQueryService) : IUserResolver
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    
    public async Task<TypedResult<UserEntity>> ResolveAsync(BackchannelAuthenticationCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(command.LoginHint))
        {
            return TypedResult<UserEntity>.Fail(new Error
            {
                Code = ErrorCode.InvalidRequest,
                Description = "login_hint is invalid"
            });
        }
        
        var user = await _userQueryService.GetByLoginAsync(command.LoginHint, cancellationToken);
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