using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Security.Identity.User;

public sealed class CurrentUserAccessor(
    IUserQueryService userQueryService, 
    IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly HttpContext? _httpContext = httpContextAccessor.HttpContext;

    public async ValueTask<TypedResult<UserEntity>> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        if (_httpContext is null)
            throw new InvalidOperationException("HTTP context is not accessible");
        
        if (_httpContext.User?.Identity?.IsAuthenticated == false)
        {
            return TypedResult<UserEntity>.Fail(new Error()
            {
                Code = ErrorCode.Unauthorized,
                Description = "Unauthorized"
            });
        }

        var subjectClaim = _httpContext.User?.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null)
        {
            return TypedResult<UserEntity>.Fail(new Error()
            {
                Code = ErrorCode.Unauthorized,
                Description = "Unauthorized"
            });
        }

        var user = await _userQueryService.GetBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null)
        {
            return TypedResult<UserEntity>.Fail(new Error()
            {
                Code = ErrorCode.Unauthorized,
                Description = "Unauthorized"
            });
        }

        return TypedResult<UserEntity>.Success(user);
    }
}