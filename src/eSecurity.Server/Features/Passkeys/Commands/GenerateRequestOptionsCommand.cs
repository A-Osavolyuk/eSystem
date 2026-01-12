using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Credentials.PublicKey;
using eSecurity.Core.Security.Credentials.PublicKey.Constants;
using eSecurity.Server.Common.Storage.Session;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Credentials.PublicKey.Challenge;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Http.Context;
using CredentialOptions = eSecurity.Server.Security.Credentials.PublicKey.Credentials.CredentialOptions;

namespace eSecurity.Server.Features.Passkeys.Commands;

public record GenerateRequestOptionsCommand(GenerateRequestOptionsRequest Request) : IRequest<Result>;

public class GenerateRequestOptionsCommandHandler(
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor,
    ISessionStorage sessionStorage,
    IChallengeFactory challengeFactory,
    IDeviceManager deviceManager,
    IPasskeyManager passkeyManager,
    IOptions<CredentialOptions> options) : IRequestHandler<GenerateRequestOptionsCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ISessionStorage _sessionStorage = sessionStorage;
    private readonly IChallengeFactory _challengeFactory = challengeFactory;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly CredentialOptions _credentialOptions = options.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GenerateRequestOptionsCommand request, CancellationToken cancellationToken)
    {
        var challenge = _challengeFactory.Create();
        var options = new PublicKeyCredentialRequestOptions()
        {
            Challenge = challenge,
            Timeout = _credentialOptions.Timeout,
            Domain = _credentialOptions.Domain,
            UserVerification = UserVerifications.Required,
        };

        if (request.Request.UserId.HasValue)
        {
            var user = await _userManager.FindByIdAsync(request.Request.UserId.Value, cancellationToken);
            if (user is null) return Results.NotFound("User not found.");

            var userAgent = _httpContext.GetUserAgent();
            var ipAddress = _httpContext.GetIpV4();
            var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
            if (device is null)
            {
                return Results.BadRequest(new Error()
                {
                    Code = Errors.Common.InvalidDevice,
                    Description = "Invalid device."
                });
            }

            var passkey = await _passkeyManager.FindByDeviceAsync(device, cancellationToken);
            if (passkey is null)
            {
                return Results.BadRequest(new Error()
                {
                    Code = Errors.Common.InvalidDevice,
                    Description = "This device does not have a passkey."
                });
            }

            options.AllowCredentials =
            [
                new PublicKeyCredentialDescriptor
                {
                    Type = KeyType.PublicKey,
                    Id = passkey.CredentialId,
                    Transports = [CredentialTransports.Internal]
                }
            ];
        }

        _sessionStorage.Set(ChallengeSessionKeys.Assertion, challenge);
        return Results.Ok(options);
    }
}