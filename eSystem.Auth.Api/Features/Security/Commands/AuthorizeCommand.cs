using eSystem.Application.Common.Http;
using eSystem.Auth.Api.Security.Authentication.SSO;
using eSystem.Auth.Api.Security.Session;
using eSystem.Domain.Requests.Auth;
using eSystem.Domain.Responses.Auth;

namespace eSystem.Auth.Api.Features.Security.Commands;

public record AuthorizeCommand(AuthorizeRequest Request) : IRequest<Result>;

public class AuthorizeCommandHandler(
    IUserManager userManager,
    ITokenManager tokenManager,
    IHttpContextAccessor httpContextAccessor,
    ISessionManager sessionManager,
    IClientManager clientManager) : IRequestHandler<AuthorizeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly ISessionManager sessionManager = sessionManager;
    private readonly IClientManager clientManager = clientManager;

    public async Task<Result> Handle(AuthorizeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var userAgent = httpContextAccessor.HttpContext?.GetUserAgent()!;
        var ipAddress = httpContextAccessor.HttpContext?.GetIpV4()!;
        
        var device = user.GetDevice(userAgent, ipAddress);
        if (device is null) return Results.NotFound("Invalid device.");

        var session = await sessionManager.FindAsync(device, cancellationToken);
        if (session is null) return Results.NotFound("Invalid authorization session.");
        
        var client = await clientManager.FindByClientIdAsync(request.Request.ClientId, cancellationToken);
        if (client is null) return Results.NotFound("Client not found.");
        
        var accessToken = tokenManager.GenerateAccessToken(user);
        var refreshToken = tokenManager.GenerateRefreshToken();
        
        var result = await tokenManager.SaveAsync(session, client, refreshToken, cancellationToken);
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