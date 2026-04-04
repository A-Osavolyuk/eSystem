using eSecurity.Server.Security.Authentication.TwoFactor.RecoveryCode;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.TwoFactor.Commands;

public record GenerateRecoveryCodesCommand() : IRequest<Result>;

public class GenerateRecoveryCodesCommandHandler(
    IRecoverManager recoverManager,
    IHttpContextAccessor httpContextAccessor,
    IUserManager userManager) : IRequestHandler<GenerateRecoveryCodesCommand, Result>
{
    private readonly IRecoverManager _recoverManager = recoverManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly IUserManager _userManager = userManager;

    public async Task<Result> Handle(GenerateRecoveryCodesCommand request, CancellationToken cancellationToken)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null) return Results.BadRequest("Invalid request");
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null) return Results.NotFound("User not found");

        var response = await _recoverManager.GenerateAsync(user, cancellationToken);
        return Results.Ok(response);
    }
}