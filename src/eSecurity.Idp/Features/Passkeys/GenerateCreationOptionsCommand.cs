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

namespace eSecurity.Idp.Features.Passkeys;

public sealed class GenerateCreationOptionsCommand : IRequest<Result>
{
    [JsonPropertyName("display_name")] 
    public string? DisplayName { get; set; }
}

public sealed class GenerateCreationOptionsCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    ISessionStorage sessionStorage,
    ICurrentUserAccessor currentUserAccessor,
    IDeviceQueryService deviceQueryService,
    IOptions<CredentialOptions> options) : IRequestHandler<GenerateCreationOptionsCommand, Result>
{
    private readonly ISessionStorage _sessionStorage = sessionStorage;
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IDeviceQueryService _deviceQueryService = deviceQueryService;
    private readonly CredentialOptions _credentialOptions = options.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GenerateCreationOptionsCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var userAgent = _httpContext.GetUserAgent();
        var ipAddress = _httpContext.GetIpV4();
        var device = await _deviceQueryService.GetByMetadataAsync(user.Id, userAgent, ipAddress, cancellationToken);
        
        if (device is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidDevice,
                Description = "Invalid device."
            });
        }

        var challenge = ChallengeFactory.Create();
        var browser = device.Browser!.Split(" ").First();
        var identifier = $"{user.Id}_{device.Device}_{browser}";
        var identifierBytes = Encoding.UTF8.GetBytes(identifier);
        var identifierBase64 = Convert.ToBase64String(identifierBytes);
        if (string.IsNullOrWhiteSpace(request.DisplayName))
            throw new ValidationException("DisplayName is required");
        
        var options = new PublicKeyCredentialCreationOptions
        {
            Challenge = challenge,
            PublicKeyCredentialUser = new PublicKeyCredentialUser
            {
                Id = identifierBase64,
                Name = user.Username,
                DisplayName = request.DisplayName,
            },
            AuthenticatorSelection = new AuthenticatorSelection
            {
                AuthenticatorAttachment = AuthenticatorAttachment.Platform,
                UserVerification = UserVerification.Required,
                ResidentKey = ResidentKey.Preferred
            },
            PublicKeyCredentialParameters =
            [
                new PublicKeyCredentialParameter { Algorithm = Algorithm.Es256, Type = KeyType.PublicKey },
                new PublicKeyCredentialParameter { Algorithm = Algorithm.Rs256, Type = KeyType.PublicKey },
            ],
            ReplyingParty = new ReplyingParty
            {
                Domain = _credentialOptions.Domain,
                Name = _credentialOptions.Server,
            },
            Attestation = Attestation.Direct,
            Timeout = _credentialOptions.Timeout,
        };

        _sessionStorage.Set(ChallengeSessionKeys.Attestation, challenge);
        return Results.Success(SuccessCodes.Ok, options);
    }
}

public sealed class GenerateCreationOptionsCommandValidator : IRequestValidator<GenerateCreationOptionsCommand>
{
    public async ValueTask<Result> Validate(GenerateCreationOptionsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'display_name' is required"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}