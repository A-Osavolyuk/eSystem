using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.TwoFactor.Commands;

public record PreferTwoFactorMethodCommand(PreferTwoFactorMethodRequest Request) : IRequest<Result>;

public class PreferMethodCommandHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<PreferTwoFactorMethodCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(PreferTwoFactorMethodCommand request, CancellationToken cancellationToken)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null) return Results.BadRequest("Invalid request");
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");
        
        var result = await _twoFactorManager.PreferAsync(user, request.Request.PreferredMethod, cancellationToken);
        return result;
    }
}