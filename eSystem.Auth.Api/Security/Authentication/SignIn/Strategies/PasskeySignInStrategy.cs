using eSystem.Auth.Api.Security.Authentication.SSO.Session;
using eSystem.Auth.Api.Security.Credentials.PublicKey;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Responses.Auth;
using eSystem.Core.Security.Credentials.Constants;
using eSystem.Core.Security.Credentials.PublicKey;

namespace eSystem.Auth.Api.Security.Authentication.SignIn.Strategies;

public class PasskeySignInStrategy(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    ISessionManager sessionManager,
    IHttpContextAccessor accessor) : SignInStrategy
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasskeyManager passkeyManager = passkeyManager;
    private readonly ISessionManager sessionManager = sessionManager;
    private readonly HttpContext httpContext = accessor.HttpContext!;

    public override async ValueTask<Result> SignInAsync(Dictionary<string, object> credentials, 
        CancellationToken cancellationToken = default)
    {
        SignInResponse response;   
        
        var credential = credentials["Credential"] as PublicKeyCredential;
        if (credential is null) return eSystem.Core.Common.Results.Results.BadRequest("Credential is empty");
        
        var credentialId = CredentialUtils.ToBase64String(credential.Id);
        var passkey = await passkeyManager.FindByCredentialIdAsync(credentialId, cancellationToken);
        if (passkey is null) return eSystem.Core.Common.Results.Results.BadRequest("Invalid credential");

        var user = await userManager.FindByIdAsync(passkey.Device.UserId, cancellationToken);
        if (user is null) return eSystem.Core.Common.Results.Results.NotFound($"Cannot find user with ID {passkey.Device.UserId}.");

        var savedChallenge = httpContext.Session.GetString(ChallengeSessionKeys.Assertion);
        if (string.IsNullOrEmpty(savedChallenge)) return eSystem.Core.Common.Results.Results.BadRequest("Invalid challenge");

        var result = await passkeyManager.VerifyAsync(passkey, credential, savedChallenge, cancellationToken);
        if (!result.Succeeded) return result;

        if (user.LockoutState.Enabled)
        {
            response = new SignInResponse()
            {
                UserId = user.Id,
                IsLockedOut = user.LockoutState.Enabled,
            };

            return eSystem.Core.Common.Results.Results.BadRequest("Account is locked out", response);
        }

        var userAgent = httpContext.GetUserAgent()!;
        var ipAddress = httpContext.GetIpV4()!;
        var device = user.GetDevice(userAgent, ipAddress);
        if (device is null) return eSystem.Core.Common.Results.Results.NotFound($"Invalid device.");
        
        await sessionManager.CreateAsync(device, cancellationToken);

        response = new SignInResponse() { UserId = user.Id, };
        return Result.Success(response);
    }
}