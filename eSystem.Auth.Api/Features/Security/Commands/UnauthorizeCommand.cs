using eSystem.Application.Common.Http;
using eSystem.Auth.Api.Interfaces;
using eSystem.Auth.Api.Security.Session;
using eSystem.Domain.Requests.Auth;

namespace eSystem.Auth.Api.Features.Security.Commands;

public record UnauthorizeCommand(UnauthorizeRequest Request) : IRequest<Result>;

public class UnauthorizeCommandHandler(
    IUserManager userManager,
    ITokenManager tokenManager,
    ISessionManager sessionManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<UnauthorizeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly ISessionManager sessionManager = sessionManager;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    public async Task<Result> Handle(UnauthorizeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var userAgent = httpContextAccessor.HttpContext?.GetUserAgent()!;
        var ipAddress = httpContextAccessor.HttpContext?.GetIpV4()!;
        var device = user.GetDevice(userAgent, ipAddress);
        if (device is null) return Results.NotFound("Invalid device.");
        
        var refreshToken = await tokenManager.FindAsync(request.Request.RefreshToken, cancellationToken);
        if (refreshToken is null) return Results.BadRequest("Invalid token.");

        var session = await sessionManager.FindAsync(device, cancellationToken);
        if (session is null) return Results.NotFound("Invalid authorization session.");

        var tokenRemoveResult = await tokenManager.RemoveAsync(refreshToken, cancellationToken);
        if (!tokenRemoveResult.Succeeded) return tokenRemoveResult;

        var sessionRemoveResult = await sessionManager.RemoveAsync(session, cancellationToken);
        return sessionRemoveResult;
    }
}