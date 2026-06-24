using System.Text.Json.Serialization;
using eSecurity.Idp.Common.Storage.Session;
using eSecurity.Idp.Security.Authorization.Devices;
using eSecurity.Idp.Security.Credentials.PublicKey.Challenge;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.WebAuthN;
using eSecurity.WebAuthN.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using Credentials_CredentialOptions = eSecurity.Idp.Security.Credentials.PublicKey.Credentials.CredentialOptions;

namespace eSecurity.Idp.Features.Passkeys;

public record GenerateCreationOptionsCommand : IRequest<Result>
{
    [JsonPropertyName("display_name")]
    public required string DisplayName { get; set; }
}

public class GenerateCreationOptionsCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    ISessionStorage sessionStorage,
    IChallengeFactory challengeFactory,
    IDeviceManager deviceManager,
    ICurrentUserAccessor currentUserAccessor,
    IOptions<Credentials_CredentialOptions> options) : IRequestHandler<GenerateCreationOptionsCommand, Result>
{
    private readonly ISessionStorage _sessionStorage = sessionStorage;
    private readonly IChallengeFactory _challengeFactory = challengeFactory;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly Credentials_CredentialOptions _credentialOptions = options.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GenerateCreationOptionsCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
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

        var challenge = _challengeFactory.Create();
        var displayName = request.DisplayName;
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
        return Results.Success(SuccessCodes.Ok, options);
    }
}