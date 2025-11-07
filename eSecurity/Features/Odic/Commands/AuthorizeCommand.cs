using eSecurity.Common.Responses;
using eSecurity.Data.Entities;
using eSecurity.Security.Authentication.Odic.Client;
using eSecurity.Security.Authentication.Odic.Code;
using eSecurity.Security.Authentication.Odic.Session;
using eSecurity.Security.Identity.User;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Security.Authentication.Odic.Constants;

namespace eSecurity.Features.Odic.Commands;

public class AuthorizeCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
    public string ResponseType { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Nonce { get; set; } = string.Empty;
    public string? CodeChallenge { get; set; }
    public string? CodeChallengeMethod { get; set; }
    public List<string> Scopes { get; set; } = [];
}

public class AuthorizeCommandHandler(
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor,
    ISessionManager sessionManager,
    IAuthorizationCodeManager authorizationCodeManager,
    IClientManager clientManager) : IRequestHandler<AuthorizeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly ISessionManager sessionManager = sessionManager;
    private readonly IAuthorizationCodeManager authorizationCodeManager = authorizationCodeManager;
    private readonly IClientManager clientManager = clientManager;

    public async Task<Result> Handle(AuthorizeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var userAgent = httpContextAccessor.HttpContext?.GetUserAgent()!;
        var ipAddress = httpContextAccessor.HttpContext?.GetIpV4()!;

        var device = user.GetDevice(userAgent, ipAddress);
        if (device is null) return Results.NotFound("Invalid device.");

        var session = await sessionManager.FindAsync(device, cancellationToken);
        if (session is null) return Results.NotFound("Invalid authorization session.");

        var client = await clientManager.FindByClientIdAsync(request.ClientId, cancellationToken);
        if (client is null) return Results.NotFound("Client not found.");
        if (!client.HasRedirectUri(request.RedirectUri)) return Results.BadRequest("Invalid redirect URI.");
        if (!client.HasScopes(request.Scopes)) return Results.BadRequest("Invalid scopes.");
        if (string.IsNullOrWhiteSpace(request.Nonce)) return Results.BadRequest("Invalid nonce.");

        if (client is { Type: ClientType.Public, RequirePkce: true })
        {
            if (string.IsNullOrEmpty(request.CodeChallenge))
                return Results.BadRequest("Code challenge is required.");

            if (string.IsNullOrEmpty(request.CodeChallengeMethod))
                return Results.BadRequest("Code challenge method is required.");
        }

        if (request.ResponseType != ResponseTypes.Code) 
            return Results.BadRequest("Invalid response type.");

        var code = authorizationCodeManager.Generate();
        var authorizationCode = new AuthorizationCodeEntity()
        {
            Id = Guid.CreateVersion7(),
            ClientId = client.Id,
            DeviceId = device.Id,
            Code = code,
            Nonce = request.Nonce,
            CodeChallenge = request.CodeChallenge,
            CodeChallengeMethod = request.CodeChallengeMethod,
            RedirectUri = request.RedirectUri,
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
            State = request.State,
            Code = code
        };

        return Result.Success(response);
    }
}