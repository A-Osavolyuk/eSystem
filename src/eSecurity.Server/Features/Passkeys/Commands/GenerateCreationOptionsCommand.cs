using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Credentials.PublicKey;
using eSecurity.Core.Security.Credentials.PublicKey.Constants;
using eSecurity.Server.Common.Storage.Session;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Credentials.PublicKey.Challenge;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Http.Results;
using CredentialOptions = eSecurity.Server.Security.Credentials.PublicKey.Credentials.CredentialOptions;

namespace eSecurity.Server.Features.Passkeys.Commands;

public record GenerateCreationOptionsCommand(GenerateCreationOptionsRequest Request) : IRequest<Result>;

public class GenerateCreationOptionsCommandHandler(
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor,
    ISessionStorage sessionStorage,
    IChallengeFactory challengeFactory,
    IDeviceManager deviceManager,
    IOptions<CredentialOptions> options) : IRequestHandler<GenerateCreationOptionsCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ISessionStorage _sessionStorage = sessionStorage;
    private readonly IChallengeFactory _challengeFactory = challengeFactory;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly CredentialOptions _credentialOptions = options.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GenerateCreationOptionsCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found");

        var userAgent = _httpContext.GetUserAgent();
        var ipAddress = _httpContext.GetIpV4();
        var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidDevice,
                Description = "Invalid device."
            });
        }

        var challenge = _challengeFactory.Create();
        var displayName = request.Request.DisplayName;
        var browser = device.Browser!.Split(" ").First();
        var identifier = $"{user.Id}_{device.Device}_{browser}";
        var identifierBytes = Encoding.UTF8.GetBytes(identifier);
        var identifierBase64 = Convert.ToBase64String(identifierBytes);
        var options = new PublicKeyCredentialCreationOptions
        {
            Challenge = challenge,
            PublicKeyCredentialUser = new PublicKeyCredentialUser
            {
                Id = identifierBase64,
                Name = user.Username,
                DisplayName = displayName,
            },
            AuthenticatorSelection = new AuthenticatorSelection
            {
                AuthenticatorAttachment = AuthenticatorAttachments.Platform,
                UserVerification = UserVerifications.Required,
                ResidentKey = ResidentKeys.Preferred
            },
            PublicKeyCredentialParameters =
            [
                new PublicKeyCredentialParameter { Algorithm = Algorithms.Es256, Type = KeyType.PublicKey },
                new PublicKeyCredentialParameter { Algorithm = Algorithms.Rs256, Type = KeyType.PublicKey },
            ],
            ReplyingParty = new ReplyingParty
            {
                Domain = _credentialOptions.Domain,
                Name = _credentialOptions.Server,
            },
            Attestation = Attestations.Direct,
            Timeout = _credentialOptions.Timeout,
        };

        _sessionStorage.Set(ChallengeSessionKeys.Attestation, challenge);
        return Results.Ok(options);
    }
}