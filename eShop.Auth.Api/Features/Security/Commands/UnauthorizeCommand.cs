using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record UnauthorizeCommand(UnauthorizeRequest Request) : IRequest<Result>;

public class UnauthorizeCommandHandler(
    IUserManager userManager,
    ITokenManager tokenManager,
    IAuthorizationManager authorizationManager,
    IDeviceManager deviceManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<UnauthorizeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IAuthorizationManager authorizationManager = authorizationManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    public async Task<Result> Handle(UnauthorizeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var userAgent = httpContextAccessor.HttpContext?.GetUserAgent()!;
        var ipV4 = httpContextAccessor.HttpContext?.GetIpV4()!;

        var device = await deviceManager.FindAsync(user, userAgent, ipV4, cancellationToken);
        if (device is null) return Results.NotFound("Invalid device.");
        
        var refreshToken = await tokenManager.FindAsync(device, cancellationToken);
        if (refreshToken is null) return Results.BadRequest("Invalid token.");

        var session = await authorizationManager.FindAsync(device, cancellationToken);
        if (session is null) return Results.NotFound("Invalid authorization session.");

        var tokenRemoveResult = await tokenManager.RemoveAsync(refreshToken, cancellationToken);
        if (!tokenRemoveResult.Succeeded) return tokenRemoveResult;

        var sessionRemoveResult = await authorizationManager.RemoveAsync(session, cancellationToken);
        return sessionRemoveResult;
    }
}