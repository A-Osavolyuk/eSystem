using eSystem.Auth.Api.Security.Authentication.SSO.Session;
using eSystem.Auth.Api.Security.Authentication.Tokens.Jwt;
using eSystem.Auth.Api.Security.Identity.User;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Requests.Auth;

namespace eSystem.Auth.Api.Features.SSO.Commands;

public record SignOutCommand(SignOutRequest Request) : IRequest<Result>;

public class SignOutCommandHandler(
    IUserManager userManager,
    ITokenManager tokenManager,
    ISessionManager sessionManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<SignOutCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly ISessionManager sessionManager = sessionManager;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    public async Task<Result> Handle(SignOutCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var userAgent = httpContextAccessor.HttpContext?.GetUserAgent()!;
        var ipAddress = httpContextAccessor.HttpContext?.GetIpV4()!;
        var device = user.GetDevice(userAgent, ipAddress);
        if (device is null) return Results.NotFound("Invalid device.");
        
        var refreshToken = await tokenManager.FindByTokenAsync(request.Request.RefreshToken, cancellationToken);
        if (refreshToken is null) return Results.BadRequest("Invalid token.");

        var session = await sessionManager.FindAsync(device, cancellationToken);
        if (session is null) return Results.NotFound("Invalid authorization session.");

        var tokenRemoveResult = await tokenManager.RemoveAsync(refreshToken, cancellationToken);
        if (!tokenRemoveResult.Succeeded) return tokenRemoveResult;

        var sessionRemoveResult = await sessionManager.RemoveAsync(session, cancellationToken);
        return sessionRemoveResult;
    }
}