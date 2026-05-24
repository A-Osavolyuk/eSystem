using eSecurity.Idp.Common.Storage.Session;
using eSecurity.Idp.Security.Authorization.Devices;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Credentials.PublicKey.Challenge;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Requests;
using eSecurity.WebAuthN;
using eSecurity.WebAuthN.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using Credentials_CredentialOptions = eSecurity.Idp.Security.Credentials.PublicKey.Credentials.CredentialOptions;

namespace eSecurity.Idp.Features.Passkeys.Commands;

public record GenerateRequestOptionsCommand(GenerateRequestOptionsRequest Request) : IRequest<Result>;

public class GenerateRequestOptionsCommandHandler(
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor,
    ISessionStorage sessionStorage,
    IChallengeFactory challengeFactory,
    IDeviceManager deviceManager,
    IPasskeyManager passkeyManager,
    IOptions<Credentials_CredentialOptions> options) : IRequestHandler<GenerateRequestOptionsCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ISessionStorage _sessionStorage = sessionStorage;
    private readonly IChallengeFactory _challengeFactory = challengeFactory;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly Credentials_CredentialOptions _credentialOptions = options.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GenerateRequestOptionsCommand request, CancellationToken cancellationToken)
    {
        var challenge = _challengeFactory.Create();
        var options = new PublicKeyCredentialRequestOptions
        {
            Challenge = challenge,
            Timeout = _credentialOptions.Timeout,
            Domain = _credentialOptions.Domain,
            UserVerification = UserVerifications.Required,
        };

        if (!string.IsNullOrEmpty(request.Request.Subject))
        {
            var user = await _userManager.FindBySubjectAsync(request.Request.Subject, cancellationToken);
            if (user is null)
            {
                return Results.ClientError(ClientErrorCode.NotFound, new Error
                {
                    Code = ErrorCode.NotFound,
                    Description = "User not found."
                });
            }

            var userAgent = _httpContext.GetUserAgent();
            var ipAddress = _httpContext.GetIpV4();
            var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
            if (device is null)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidDevice,
                    Description = "Invalid device."
                });
            }

            var passkey = await _passkeyManager.FindByDeviceAsync(device, cancellationToken);
            if (passkey is null)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidDevice,
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
        return Results.Success(SuccessCodes.Ok, options);
    }
}