using eShop.Application.Http;
using eShop.Auth.Api.Security.Credentials.PublicKey;
using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Common.Security.Credentials;
using eShop.Domain.Responses.Auth;

namespace eShop.Auth.Api.Security.Authentication.SignIn.Strategies;

public class PasskeySignInStrategy(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    ILoginManager loginManager,
    IAuthorizationManager authorizationManager,
    IHttpContextAccessor accessor) : SignInStrategy
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasskeyManager passkeyManager = passkeyManager;
    private readonly ILoginManager loginManager = loginManager;
    private readonly IAuthorizationManager authorizationManager = authorizationManager;
    private readonly HttpContext httpContext = accessor.HttpContext!;

    public override async ValueTask<Result> SignInAsync(Dictionary<string, object> credentials, 
        CancellationToken cancellationToken = default)
    {
        SignInResponse response;   
        
        var credential = credentials["Credential"] as PublicKeyCredential;
        if (credential is null) return Domain.Common.API.Results.BadRequest("Credential is empty");
        
        var credentialId = CredentialUtils.ToBase64String(credential.Id);
        var passkey = await passkeyManager.FindByCredentialIdAsync(credentialId, cancellationToken);
        if (passkey is null) return Domain.Common.API.Results.BadRequest("Invalid credential");

        var user = await userManager.FindByIdAsync(passkey.Device.UserId, cancellationToken);
        if (user is null) return Domain.Common.API.Results.NotFound($"Cannot find user with ID {passkey.Device.UserId}.");

        var savedChallenge = httpContext.Session.GetString(ChallengeSessionKeys.Assertion);
        if (string.IsNullOrEmpty(savedChallenge)) return Domain.Common.API.Results.BadRequest("Invalid challenge");

        var result = await passkeyManager.VerifyAsync(passkey, credential, savedChallenge, cancellationToken);
        if (!result.Succeeded) return result;

        if (user.LockoutState.Enabled)
        {
            response = new SignInResponse()
            {
                UserId = user.Id,
                IsLockedOut = user.LockoutState.Enabled,
            };

            return Domain.Common.API.Results.BadRequest("Account is locked out", response);
        }

        var userAgent = httpContext.GetUserAgent()!;
        var ipAddress = httpContext.GetIpV4()!;
        var device = user.GetDevice(userAgent, ipAddress);
        if (device is null) return Domain.Common.API.Results.NotFound($"Invalid device.");

        await loginManager.CreateAsync(device, LoginType.Passkey, cancellationToken);
        await authorizationManager.CreateAsync(device, cancellationToken);

        response = new SignInResponse() { UserId = user.Id, };
        return Result.Success(response);
    }
}