using eSystem.Auth.Api.Security.Authentication.SSO;
using eSystem.Auth.Api.Security.Authentication.SSO.Client;
using eSystem.Auth.Api.Security.Authentication.SSO.Code;
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
    ISessionStorage sessionStorage,
    IAuthorizationCodeManager authorizationCodeManager,
    IClientManager clientManager) : IRequestHandler<AuthorizeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly ISessionManager sessionManager = sessionManager;
    private readonly ISessionStorage sessionStorage = sessionStorage;
    private readonly IAuthorizationCodeManager authorizationCodeManager = authorizationCodeManager;
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

        if (string.IsNullOrWhiteSpace(request.Request.Nonce)) return Results.BadRequest("Invalid nonce.");
        sessionStorage.Set(SessionKeys.Nonce, request.Request.Nonce);
        
        var code = authorizationCodeManager.Generate();
        
        var authorizationCode = new AuthorizationCodeEntity()
        {
            Id = Guid.CreateVersion7(),
            ClientId = client.Id,
            DeviceId = device.Id,
            Code = code,
            RedirectUri = request.Request.RedirectUri,
            ExpireDate = DateTimeOffset.UtcNow.AddMinutes(10),
            CreateDate = DateTimeOffset.UtcNow,
        };
        
        var codeResult = await authorizationCodeManager.CreateAsync(authorizationCode, cancellationToken);
        if (!codeResult.Succeeded) return Results.BadRequest(codeResult);
        
        var response = new AuthorizeResponse()
        {
            UserId = user.Id,
            SessionId = session.Id,
            DeviceId = device.Id,
            State = request.Request.State,
            Code = code
        };
        
        return Result.Success(response);
    }
}