using eShop.Domain.Requests.Auth;
using eShop.Domain.Responses.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<Result>;

public class RefreshTokenCommandHandler(
    ITokenManager tokenManager,
    IUserManager userManager,
    IDeviceManager deviceManager,
    IHttpContextAccessor httpContextAccessor,
    IAuthorizationManager authorizationManager) : IRequestHandler<RefreshTokenCommand, Result>
{
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly IAuthorizationManager authorizationManager = authorizationManager;

    public async Task<Result> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with id {request.Request.UserId}.");
        
        var userAgent = httpContextAccessor.HttpContext?.GetUserAgent()!;
        var ipV4 = httpContextAccessor.HttpContext?.GetIpV4()!;

        var device = await deviceManager.FindAsync(user, userAgent, ipV4, cancellationToken);
        if (device is null) return Results.NotFound("Invalid device.");
        
        var refreshToken = await tokenManager.FindAsync(device, cancellationToken);
        if (refreshToken is null) return Results.BadRequest("Invalid token.");
        
        var session = await authorizationManager.FindAsync(device, cancellationToken);
        if (session is null) return Results.NotFound("Invalid authorization session.");

        var accessToken = await tokenManager.GenerateAsync(device, cancellationToken);
        var response = new RefreshTokenResponse()
        {
            UserId = user.Id,
            AccessToken = accessToken
        };

        return Result.Success(response);
    }
}