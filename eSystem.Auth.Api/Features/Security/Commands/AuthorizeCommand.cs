using eSystem.Application.Common.Http;
using eSystem.Auth.Api.Interfaces;
using eSystem.Domain.Requests.Auth;
using eSystem.Domain.Responses.Auth;

namespace eSystem.Auth.Api.Features.Security.Commands;

public record AuthorizeCommand(AuthorizeRequest Request) : IRequest<Result>;

public class AuthorizeCommandHandler(
    IUserManager userManager,
    ITokenManager tokenManager,
    IHttpContextAccessor httpContextAccessor,
    IAuthorizationManager authorizationManager) : IRequestHandler<AuthorizeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly IAuthorizationManager authorizationManager = authorizationManager;

    public async Task<Result> Handle(AuthorizeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var userAgent = httpContextAccessor.HttpContext?.GetUserAgent()!;
        var ipAddress = httpContextAccessor.HttpContext?.GetIpV4()!;
        var device = user.GetDevice(userAgent, ipAddress);
        if (device is null) return Results.NotFound("Invalid device.");

        var session = await authorizationManager.FindAsync(device, cancellationToken);
        if (session is null) return Results.NotFound("Invalid authorization session.");
        
        var accessToken = tokenManager.GenerateAccessToken(user);
        var refreshToken = tokenManager.GenerateRefreshToken();
        
        var result = await tokenManager.SaveAsync(device, refreshToken, cancellationToken);
        if (!result.Succeeded) return result;

        var response = new AuthorizeResponse()
        {
            UserId = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        
        return Result.Success(response);
    }
}