using eSystem.Application.Common.Http;
using eSystem.Auth.Api.Interfaces;
using eSystem.Domain.Requests.Auth;
using eSystem.Domain.Responses.Auth;

namespace eSystem.Auth.Api.Features.Security.Commands;

public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<Result>;

public class RefreshTokenCommandHandler(
    ITokenManager tokenManager,
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor,
    IAuthorizationManager authorizationManager) : IRequestHandler<RefreshTokenCommand, Result>
{
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly IAuthorizationManager authorizationManager = authorizationManager;

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

        var session = await authorizationManager.FindAsync(device, cancellationToken);
        if (session is null) return Results.NotFound("Invalid authorization session.");

        var accessToken = tokenManager.GenerateAccessToken(user);
        var response = new RefreshTokenResponse()
        {
            UserId = user.Id,
            AccessToken = accessToken
        };

        return Result.Success(response);
    }
}