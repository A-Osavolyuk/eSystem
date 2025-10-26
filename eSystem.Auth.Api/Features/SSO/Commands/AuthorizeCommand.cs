using eSystem.Auth.Api.Security.Authentication.SSO;
using eSystem.Auth.Api.Security.Authentication.SSO.Client;
using eSystem.Auth.Api.Security.Authentication.SSO.Session;
using eSystem.Core.Common.Http;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;

namespace eSystem.Auth.Api.Features.SSO.Commands;

public record AuthorizeCommand(AuthorizeRequest Request) : IRequest<Result>;

public class AuthorizeCommandHandler(
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor,
    ISessionManager sessionManager,
    IClientManager clientManager) : IRequestHandler<AuthorizeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
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
        if (!client.HasUri(request.Request.RedirectUri)) return Results.BadRequest("Invalid redirect URI.");
        if (!client.HasScopes(request.Request.Scopes)) return Results.BadRequest("Invalid scopes.");
        
        var response = new AuthorizeResponse()
        {
            UserId = user.Id,
            SessionId = session.Id,
            DeviceId = device.Id,
            ClientId = client.ClientId,
            State = request.Request.State,
            Nonce = request.Request.Nonce,
            RedirectUri = request.Request.RedirectUri
        };
        
        return Result.Success(response);
    }
}