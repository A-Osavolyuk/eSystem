using eSecurity.Security.Authentication.ODIC.Session;
using eSecurity.Security.Credentials.PublicKey;
using eSecurity.Security.Credentials.PublicKey.Credentials;
using eSecurity.Security.Identity.User;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Responses.Auth;
using eSystem.Core.Security.Authentication.SignIn;
using eSystem.Core.Security.Authentication.SignIn.Payloads;
using eSystem.Core.Security.Credentials.Constants;

namespace eSecurity.Security.Authentication.SignIn.Strategies;

public sealed class PasskeySignInStrategy(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    ISessionManager sessionManager,
    IHttpContextAccessor accessor) : SignInStrategy
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasskeyManager passkeyManager = passkeyManager;
    private readonly ISessionManager sessionManager = sessionManager;
    private readonly HttpContext httpContext = accessor.HttpContext!;

    public override async ValueTask<Result> SignInAsync(SignInPayload payload,
        CancellationToken cancellationToken = default)
    {
        SignInResponse response;

        if (payload is not PasskeySignInPayload passkeyPayload)
            return Results.BadRequest("Invalid payload type");

        var credential = passkeyPayload.Credential;
        var credentialId = CredentialUtils.ToBase64String(credential.Id);
        var passkey = await passkeyManager.FindByCredentialIdAsync(credentialId, cancellationToken);
        if (passkey is null) return Results.BadRequest("Invalid credential");

        var user = await userManager.FindByIdAsync(passkey.Device.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {passkey.Device.UserId}.");

        var savedChallenge = httpContext.Session.GetString(ChallengeSessionKeys.Assertion);
        if (string.IsNullOrEmpty(savedChallenge)) return Results.BadRequest("Invalid challenge");

        var result = await passkeyManager.VerifyAsync(passkey, credential, savedChallenge, cancellationToken);
        if (!result.Succeeded) return result;

        if (user.LockoutState.Enabled)
        {
            response = new SignInResponse()
            {
                UserId = user.Id,
                IsLockedOut = user.LockoutState.Enabled,
            };

            return Results.BadRequest("Account is locked out", response);
        }

        var userAgent = httpContext.GetUserAgent()!;
        var ipAddress = httpContext.GetIpV4()!;
        var device = user.GetDevice(userAgent, ipAddress);
        if (device is null) return Results.NotFound($"Invalid device.");

        await sessionManager.CreateAsync(device, cancellationToken);

        response = new SignInResponse() { UserId = user.Id, };
        return Result.Success(response);
    }
}