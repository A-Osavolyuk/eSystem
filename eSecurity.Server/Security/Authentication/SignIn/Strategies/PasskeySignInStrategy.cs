using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Core.Security.Credentials.PublicKey.Constants;
using eSecurity.Server.Data;
using eSecurity.Server.Security.Authentication.Oidc.Session;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Credentials.PublicKey.Credentials;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Http.Context;

namespace eSecurity.Server.Security.Authentication.SignIn.Strategies;

public sealed class PasskeySignInStrategy(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    ISessionManager sessionManager,
    IDeviceManager deviceManager,
    IHttpContextAccessor accessor) : ISignInStrategy
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly HttpContext _httpContext = accessor.HttpContext!;

    public async ValueTask<Result> ExecuteAsync(SignInPayload payload,
        CancellationToken cancellationToken = default)
    {
        if (payload is not PasskeySignInPayload passkeyPayload)
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.InvalidPayloadType, 
                Description = "Invalid payload type"
            });

        var credential = passkeyPayload.Credential;
        var credentialId = CredentialUtils.ToBase64String(credential.Id);
        var passkey = await _passkeyManager.FindByCredentialIdAsync(credentialId, cancellationToken);
        if (passkey is null) return Results.BadRequest("Invalid credential");

        var user = await _userManager.FindByIdAsync(passkey.Device.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {passkey.Device.UserId}.");

        var savedChallenge = _httpContext.Session.GetString(ChallengeSessionKeys.Assertion);
        if (string.IsNullOrEmpty(savedChallenge)) return Results.BadRequest("Invalid challenge");

        var result = await _passkeyManager.VerifyAsync(passkey, credential, savedChallenge, cancellationToken);
        if (!result.Succeeded) return result;

        if (user.LockoutState.Enabled)
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.AccountLockedOut, 
                Description = "Account is locked out",
                Details = new() { { "userId", user.Id } }
            });

        var userAgent = _httpContext.GetUserAgent()!;
        var ipAddress = _httpContext.GetIpV4()!;
        var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null) return Results.NotFound(new Error()
        {
            Code = Errors.Common.InvalidDevice, 
            Description = "Invalid device."
        });

        await _sessionManager.CreateAsync(device, cancellationToken);

        var response = new SignInResponse() { UserId = user.Id, };
        return Results.Ok(response);
    }
}