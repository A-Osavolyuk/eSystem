using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Credentials.PublicKey.Constants;
using eSecurity.Server.Common.Storage.Session;
using eSecurity.Server.Data;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Credentials.PublicKey.Challenge;
using eSecurity.Server.Security.Credentials.PublicKey.Credentials;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Http.Context;

namespace eSecurity.Server.Features.Passkeys.Commands;

public record GenerateRequestOptionsCommand(GenerateRequestOptionsRequest Request) : IRequest<Result>;

public class GenerateRequestOptionsCommandHandler(
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor,
    ISessionStorage sessionStorage,
    IChallengeFactory challengeFactory,
    ICredentialFactory credentialFactory,
    IDeviceManager deviceManager,
    IOptions<CredentialOptions> options) : IRequestHandler<GenerateRequestOptionsCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ISessionStorage _sessionStorage = sessionStorage;
    private readonly IChallengeFactory _challengeFactory = challengeFactory;
    private readonly ICredentialFactory _credentialFactory = credentialFactory;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly CredentialOptions _credentialOptions = options.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GenerateRequestOptionsCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByUsernameAsync(request.Request.Username!, cancellationToken);

        if (user is null)
        {
            user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
            if (user is null) return Results.NotFound("Cannot find user.");
        }

        var userAgent = _httpContext.GetUserAgent();
        var ipAddress = _httpContext.GetIpV4();
        var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null) return Results.BadRequest("Invalid device.");
        if (device.Passkey is null) return Results.BadRequest("This device does not have a passkey.");

        var challenge = _challengeFactory.Create();
        var options = _credentialFactory.CreateRequestOptions(device.Passkey, challenge, _credentialOptions);

        _sessionStorage.Set(ChallengeSessionKeys.Assertion, challenge);
        return Results.Ok(options);
    }
}