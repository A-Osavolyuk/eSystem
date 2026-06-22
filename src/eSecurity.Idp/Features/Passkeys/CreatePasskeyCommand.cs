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
using CredentialOptions = eSecurity.Idp.Security.Credentials.PublicKey.Credentials.CredentialOptions;

namespace eSecurity.Idp.Features.Passkeys;

public record CreatePasskeyCommand : IRequest<Result>
{
    [JsonPropertyName("display_name")]
    public required string DisplayName { get; set; }
    
    [JsonPropertyName("response")]
    public required PublicKeyCredentialCreationResponse Response { get; set; }
}

public class CreatePasskeyCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    IPasskeyManager passkeyManager,
    ITwoFactorManager twoFactorManager,
    IHttpContextAccessor httpContextAccessor,
    ISessionStorage sessionStorage,
    IDeviceManager deviceManager,
    IOptions<CredentialOptions> options) : IRequestHandler<CreatePasskeyCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly ISessionStorage _sessionStorage = sessionStorage;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly CredentialOptions _credentialOptions = options.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(CreatePasskeyCommand request,
        CancellationToken cancellationToken)
    {
        var userResult = await _currentUserAccessor.GetCurrentUserAsync(cancellationToken);
        if (!userResult.Succeeded)
        {
            var error = userResult.GetError();
            return Results.ClientError(ClientErrorCode.Unauthorized, error);
        }

        if (!userResult.TryGetValue(out var user))
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error()
            {
                Code = ErrorCode.Unauthorized,
                Description = "Unauthorized"
            });
        }

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

        var credentialResponse = request.Response;
        var clientData = ClientData.Parse(credentialResponse.Response.ClientDataJson);
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

        var authData = AuthenticationData.Parse(credentialResponse.Response.AttestationObject);
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

        var passkey = new PasskeyEntity
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

        var result = await _passkeyManager.CreateAsync(passkey, cancellationToken);
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