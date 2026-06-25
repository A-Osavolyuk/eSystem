using System.Security.Cryptography;
using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Idp.Common.Storage.Session;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Authorization.Devices;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Credentials.PublicKey.Credentials;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.WebAuthN;
using eSecurity.WebAuthN.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Server.Exceptions;
using CredentialOptions = eSecurity.Idp.Security.Credentials.PublicKey.Credentials.CredentialOptions;

namespace eSecurity.Idp.Features.Passkeys;

public record CreateSoftwareKeyCommand : IRequest<Result>
{
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }
    
    [JsonPropertyName("response")]
    public PublicKeyCredentialCreationResponse? Response { get; set; }
}

public class CreateSoftwareKeyCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    ISoftwareKeyManager softwareKeyManager,
    ITwoFactorManager twoFactorManager,
    IHttpContextAccessor httpContextAccessor,
    ISessionStorage sessionStorage,
    IDeviceManager deviceManager,
    IOptions<CredentialOptions> options) : IRequestHandler<CreateSoftwareKeyCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly ISoftwareKeyManager _softwareKeyManager = softwareKeyManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly ISessionStorage _sessionStorage = sessionStorage;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly CredentialOptions _credentialOptions = options.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(CreateSoftwareKeyCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var userAgent = _httpContext.GetUserAgent();
        var ipAddress = _httpContext.GetIpV4();
        var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null || device.IsBlocked)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidDevice,
                Description = "Invalid device."
            });
        }
        
        if (request.Response is null)
            throw new ValidationException("Response is required");
        
        var clientData = ClientData.Parse(request.Response.Response.ClientDataJson);
        if (clientData is null || clientData.Type != ClientDataTypes.Create)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidCredentials,
                Description = "Invalid credentials."
            });
        }

        var base64Challenge = CredentialUtils.ToBase64String(clientData.Challenge);
        var savedChallenge = _sessionStorage.Get(ChallengeSessionKeys.Attestation);
        if (savedChallenge != base64Challenge)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidChallenge,
                Description = "Challenge mismatch"
            });
        }

        var authData = AuthenticationData.Parse(request.Response.Response.AttestationObject);
        var source = Encoding.UTF8.GetBytes(_credentialOptions.Domain.ToArray());
        var rpHash = SHA256.HashData(source);
        if (!authData.RpIdHash.SequenceEqual(rpHash))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidRp,
                Description = "Invalid RP ID"
            });
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName))
            throw new ValidationException("DisplayName is required");
        
        var passkey = new SoftwareKeyEntity
        {
            Id = Guid.CreateVersion7(),
            AuthenticatorId = new Guid(authData.AaGuid),
            DeviceId = device.Id,
            DisplayName = request.DisplayName,
            Domain = clientData.Origin,
            CredentialId = Convert.ToBase64String(authData.CredentialId),
            PublicKey = authData.CredentialPublicKey,
            SignCount = authData.SignCount,
            Type = request.Response.Type
        };

        var result = await _softwareKeyManager.CreateAsync(passkey, cancellationToken);
        if (!result.Succeeded) 
            return result;

        if (!await _twoFactorManager.HasMethodAsync(user, TwoFactorMethod.Passkey, cancellationToken))
        {
            var twoFactorResult = await _twoFactorManager.SubscribeAsync(user,
                TwoFactorMethod.Passkey, cancellationToken: cancellationToken);

            if (!twoFactorResult.Succeeded) 
                return twoFactorResult;
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}

public sealed class CreateSoftwareKeyCommandValidator : IRequestValidator<CreateSoftwareKeyCommand>
{
    public async ValueTask<Result> Validate(CreateSoftwareKeyCommand request, CancellationToken cancellationToken)
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

        if (request.Response is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'response' is required"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}