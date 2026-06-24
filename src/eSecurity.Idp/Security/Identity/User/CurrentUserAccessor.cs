using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Identity.Claims;
using eSystem.Core.Server.Exceptions;

namespace eSecurity.Idp.Security.Identity.User;

public sealed class CurrentUserAccessor(
    IUserQueryService userQueryService,
    IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly HttpContext? _httpContext = httpContextAccessor.HttpContext;

    public async ValueTask<UserEntity?> GetCurrentOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        if (_httpContext is null)
            return null;

        if (_httpContext.User?.Identity?.IsAuthenticated == false)
            return null;

        var subjectClaim = _httpContext.User?.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null)
            return null;

        return await _userQueryService.GetBySubjectAsync(subjectClaim.Value, cancellationToken);
    }

    public async ValueTask<UserEntity> GetRequiredCurrentAsync(CancellationToken cancellationToken = default)
    {
        if (_httpContext is null)
            throw new InvalidOperationException("HTTP context is not accessible");

        if (_httpContext.User?.Identity?.IsAuthenticated == false)
            throw new UnauthorizedException("Unauthorized");

        var subjectClaim = _httpContext.User?.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null)
            throw new UnauthorizedException("Unauthorized");

        var user = await _userQueryService.GetBySubjectAsync(subjectClaim.Value, cancellationToken);
        return user ?? throw new UnauthorizedException("Unauthorized");
    }
}