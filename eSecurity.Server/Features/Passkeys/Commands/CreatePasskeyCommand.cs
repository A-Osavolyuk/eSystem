using System.Security.Cryptography;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Core.Security.Credentials.PublicKey.Constants;
using eSecurity.Server.Common.Storage.Session;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Credentials.PublicKey.Credentials;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Http.Context;

namespace eSecurity.Server.Features.Passkeys.Commands;

public record CreatePasskeyCommand(CreatePasskeyRequest Request) : IRequest<Result>;

public class CreatePasskeyCommandHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    ITwoFactorManager twoFactorManager,
    IHttpContextAccessor httpContextAccessor,
    IVerificationManager verificationManager,
    ISessionStorage sessionStorage,
    IDeviceManager deviceManager,
    IOptions<CredentialOptions> options) : IRequestHandler<CreatePasskeyCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly ISessionStorage _sessionStorage = sessionStorage;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly CredentialOptions _credentialOptions = options.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(CreatePasskeyCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var userAgent = _httpContext.GetUserAgent();
        var ipAddress = _httpContext.GetIpV4();
        var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null) return Results.BadRequest("Invalid device.");

        var verificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.Passkey, ActionType.Create, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var credentialResponse = request.Request.Response;
        var clientData = ClientData.Parse(credentialResponse.Response.ClientDataJson);
        if (clientData is null) return Results.BadRequest("Invalid client data");
        if (clientData.Type != ClientDataTypes.Create) return Results.BadRequest("Invalid type");

        var base64Challenge = CredentialUtils.ToBase64String(clientData.Challenge);
        var savedChallenge = _sessionStorage.Get(ChallengeSessionKeys.Attestation);
        if (savedChallenge != base64Challenge) return Results.BadRequest("Challenge mismatch");

        var authData = AuthenticationData.Parse(credentialResponse.Response.AttestationObject);
        var source = Encoding.UTF8.GetBytes(_credentialOptions.Domain.ToArray());
        var rpHash = SHA256.HashData(source);
        if (!authData.RpIdHash.SequenceEqual(rpHash)) return Results.BadRequest("Invalid RP ID");

        var passkey = new PasskeyEntity()
        {
            Id = Guid.CreateVersion7(),
            AuthenticatorId = new Guid(authData.AaGuid),
            DeviceId = device.Id,
            DisplayName = request.Request.DisplayName,
            Domain = clientData.Origin,
            CredentialId = Convert.ToBase64String(authData.CredentialId),
            PublicKey = authData.CredentialPublicKey,
            SignCount = authData.SignCount,
            CreateDate = DateTimeOffset.UtcNow,
            Type = request.Request.Response.Type
        };

        var result = await _passkeyManager.CreateAsync(passkey, cancellationToken);
        if (!result.Succeeded) return result;

        if (user.TwoFactorEnabled && !user.HasTwoFactor(TwoFactorMethod.Passkey))
        {
            var twoFactorResult = await _twoFactorManager.SubscribeAsync(user,
                TwoFactorMethod.Passkey, cancellationToken: cancellationToken);

            if (!twoFactorResult.Succeeded) return twoFactorResult;
        }

        if (!user.HasVerification(VerificationMethod.Passkey))
        {
            var verificationMethodResult = await _verificationManager.SubscribeAsync(user,
                VerificationMethod.Passkey, cancellationToken: cancellationToken);

            if (!verificationMethodResult.Succeeded) return verificationMethodResult;
        }
        
        return Results.Ok();
    }
}