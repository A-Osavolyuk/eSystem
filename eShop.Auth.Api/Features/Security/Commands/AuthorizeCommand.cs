using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record AuthorizeCommand(AuthorizeRequest Request) : IRequest<Result>;

public class AuthorizeCommandHandler(
    IUserManager userManager,
    ITokenManager tokenManager,
    IDeviceManager deviceManager,
    IHttpContextAccessor httpContextAccessor,
    TokenHandler tokenHandler) : IRequestHandler<AuthorizeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly TokenHandler tokenHandler = tokenHandler;

    public async Task<Result> Handle(AuthorizeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var userAgent = httpContextAccessor.HttpContext?.GetUserAgent()!;
        var ipV4 = httpContextAccessor.HttpContext?.GetIpV4()!;

        var device = await deviceManager.FindAsync(user, userAgent, ipV4, cancellationToken);
        if (device is null) return Results.NotFound($"Invalid device.");
        
        var accessToken = await tokenManager.GenerateAsync(user, device, cancellationToken);

        var response = new AuthorizeResponse()
        {
            UserId = user.Id,
            AccessToken = accessToken,
        };
        
        return Result.Success(response);
    }
}