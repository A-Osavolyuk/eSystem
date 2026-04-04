using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.TwoFactor.Commands;

public record VerifyAuthenticatorCommand(VerifyAuthenticatorRequest Request) : IRequest<Result>;

public class VerifyAuthenticatorCommandHandler(
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<VerifyAuthenticatorCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly HttpContext _httpContext= httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(VerifyAuthenticatorCommand request, CancellationToken cancellationToken)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null) return Results.BadRequest("Invalid request");
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");
        
        var verified = AuthenticatorUtils.VerifyCode(request.Request.Code, request.Request.Secret);
        return verified ? Results.Ok() : Results.BadRequest("Invalid code.");
    }
}