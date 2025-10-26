using System.Security.Claims;
using eSystem.Auth.Api.Security.Session;
using eSystem.Core.Common.Http;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;
using eSystem.Core.Security.Claims;

namespace eSystem.Auth.Api.Features.SSO.Commands;

public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<Result>;

public class RefreshTokenCommandHandler(
    ITokenManager tokenManager,
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor,
    ISessionManager sessionManager) : IRequestHandler<RefreshTokenCommand, Result>
{
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly ISessionManager sessionManager = sessionManager;

    public async Task<Result> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with id {request.Request.UserId}.");
        
        var userAgent = httpContextAccessor.HttpContext?.GetUserAgent()!;
        var ipAddress = httpContextAccessor.HttpContext?.GetIpV4()!;
        var device = user.GetDevice(userAgent, ipAddress);
        if (device is null) return Results.NotFound("Invalid device.");
        
        var refreshToken = await tokenManager.FindAsync(request.Request.RefreshToken, cancellationToken);
        if (refreshToken is null || !refreshToken.IsValid) return Results.BadRequest("Invalid token.");

        var session = await sessionManager.FindAsync(device, cancellationToken);
        if (session is null) return Results.NotFound("Invalid authorization session.");

        var claims = new List<Claim>()
        {
            new (AppClaimTypes.Jti, Guid.NewGuid().ToString()),
            new (AppClaimTypes.Subject,  user.Id.ToString()),
        };
        
        var accessToken = tokenManager.GenerateAccessToken(claims);
        var response = new RefreshTokenResponse()
        {
            UserId = user.Id,
            AccessToken = accessToken
        };

        return Result.Success(response);
    }
}