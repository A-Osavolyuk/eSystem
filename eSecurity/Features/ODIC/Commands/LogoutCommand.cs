using eSecurity.Security.Authentication.ODIC.Session;
using eSecurity.Security.Identity.User;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Requests.Auth;

namespace eSecurity.Features.ODIC.Commands;

public record LogoutCommand(LogoutRequest Request) : IRequest<Result>;

public class LogoutCommandHandler(
    IUserManager userManager,
    ISessionManager sessionManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<LogoutCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ISessionManager sessionManager = sessionManager;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var userAgent = httpContextAccessor.HttpContext?.GetUserAgent()!;
        var ipAddress = httpContextAccessor.HttpContext?.GetIpV4()!;
        var device = user.GetDevice(userAgent, ipAddress);
        if (device is null) return Results.NotFound("Invalid device.");

        var session = await sessionManager.FindAsync(device, cancellationToken);
        if (session is null) return Results.NotFound("Invalid authorization session.");

        var sessionRemoveResult = await sessionManager.RemoveAsync(session, cancellationToken);
        return sessionRemoveResult;
    }
}