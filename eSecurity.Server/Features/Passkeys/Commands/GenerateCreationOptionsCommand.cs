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

public record GenerateCreationOptionsCommand(GenerateCreationOptionsRequest Request) : IRequest<Result>;

public class GenerateCreationOptionsCommandHandler(
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor,
    ISessionStorage sessionStorage,
    ICredentialFactory credentialFactory,
    IChallengeFactory challengeFactory,
    IDeviceManager deviceManager,
    IOptions<CredentialOptions> options) : IRequestHandler<GenerateCreationOptionsCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ISessionStorage _sessionStorage = sessionStorage;
    private readonly ICredentialFactory _credentialFactory = credentialFactory;
    private readonly IChallengeFactory _challengeFactory = challengeFactory;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly CredentialOptions _credentialOptions = options.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GenerateCreationOptionsCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var userAgent = _httpContext.GetUserAgent();
        var ipAddress = _httpContext.GetIpV4();
        var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null) return Results.BadRequest("Invalid device.");

        var challenge = _challengeFactory.Create();
        var displayName = request.Request.DisplayName;
        var browser = device.Browser!.Split(" ").First();
        var fingerprint = $"{device.Device}_{browser}";
        
        var options = _credentialFactory.CreateCreationOptions(user,
            displayName, challenge, fingerprint, _credentialOptions);

        _sessionStorage.Set(ChallengeSessionKeys.Attestation, challenge);
        return Results.Ok(options);
    }
}