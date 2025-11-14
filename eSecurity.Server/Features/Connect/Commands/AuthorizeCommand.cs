using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSecurity.Server.Security.Authentication.Oidc.Code;
using eSecurity.Server.Security.Authentication.Oidc.Session;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Security.Authentication.Oidc.Constants;

namespace eSecurity.Server.Features.Connect.Commands;

public record AuthorizeCommand(AuthorizeRequest Request) : IRequest<Result>;

public class AuthorizeCommandHandler(
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor,
    ISessionManager sessionManager,
    IAuthorizationCodeManager authorizationCodeManager,
    IClientManager clientManager) : IRequestHandler<AuthorizeCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IAuthorizationCodeManager _authorizationCodeManager = authorizationCodeManager;
    private readonly IClientManager _clientManager = clientManager;

    public async Task<Result> Handle(AuthorizeCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var userAgent = _httpContextAccessor.HttpContext?.GetUserAgent()!;
        var ipAddress = _httpContextAccessor.HttpContext?.GetIpV4()!;

        var device = user.GetDevice(userAgent, ipAddress);
        if (device is null) return Results.NotFound("Invalid device.");

        var session = await _sessionManager.FindAsync(device, cancellationToken);
        if (session is null) return Results.NotFound("Invalid authorization session.");

        var client = await _clientManager.FindByIdAsync(request.Request.ClientId, cancellationToken);
        if (client is null) return Results.NotFound("Client not found.");
        if (!client.HasRedirectUri(request.Request.RedirectUri)) return Results.BadRequest("Invalid redirect URI.");
        if (!client.HasScopes(request.Request.Scopes)) return Results.BadRequest("Invalid scopes.");
        if (string.IsNullOrWhiteSpace(request.Request.Nonce)) return Results.BadRequest("Invalid nonce.");

        if (client is { Type: ClientType.Public, RequirePkce: true })
        {
            if (string.IsNullOrEmpty(request.Request.CodeChallenge))
                return Results.BadRequest("Code challenge is required.");

            if (string.IsNullOrEmpty(request.Request.CodeChallengeMethod))
                return Results.BadRequest("Code challenge method is required.");
        }

        if (request.Request.ResponseType != ResponseTypes.Code) 
            return Results.BadRequest("Invalid response type.");

        var code = _authorizationCodeManager.Generate();
        var authorizationCode = new AuthorizationCodeEntity()
        {
            Id = Guid.CreateVersion7(),
            ClientId = client.Id,
            DeviceId = device.Id,
            Code = code,
            Nonce = request.Request.Nonce,
            CodeChallenge = request.Request.CodeChallenge,
            CodeChallengeMethod = request.Request.CodeChallengeMethod,
            RedirectUri = request.Request.RedirectUri,
            ExpireDate = DateTimeOffset.UtcNow.AddMinutes(10),
            CreateDate = DateTimeOffset.UtcNow,
        };

        var codeResult = await _authorizationCodeManager.CreateAsync(authorizationCode, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

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