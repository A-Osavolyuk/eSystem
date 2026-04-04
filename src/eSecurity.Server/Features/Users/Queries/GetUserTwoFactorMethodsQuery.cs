using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserTwoFactorMethodsQuery : IRequest<Result>;

public class GetUserProvidersQueryHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetUserTwoFactorMethodsQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GetUserTwoFactorMethodsQuery request, CancellationToken cancellationToken)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null) return Results.BadRequest("Invalid subject.");
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var methods = await _twoFactorManager.GetAllAsync(user, cancellationToken);
        var response = methods.Select(provider => new UserTwoFactorMethod
        {
            Method = provider.Method,
            Preferred = provider.Preferred,
            UpdateDate = provider.UpdateDate,
        }).ToList();
        
        return Results.Ok(response);
    }
}