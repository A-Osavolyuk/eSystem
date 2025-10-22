using eShop.Auth.Api.Security.Authentication.TwoFactor.Authenticator;
using eShop.Auth.Api.Security.Cryptography.Protection;
using eShop.Auth.Api.Security.Identity.Options;
using eShop.Domain.Responses.Auth;

namespace eShop.Auth.Api.Security.Authentication.SignIn.Strategies;

public class AuthenticatorSignInStrategy(
    IUserManager userManager,
    ILockoutManager lockoutManager,
    IDeviceManager deviceManager,
    ILoginManager loginManager,
    IAuthorizationManager authorizationManager,
    ISecretManager secretManager,
    IHttpContextAccessor accessor,
    IProtectorFactory protectorFactory,
    IOptions<SignInOptions> options) : SignInStrategy
{
    private readonly IUserManager userManager = userManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly ILoginManager loginManager = loginManager;
    private readonly IAuthorizationManager authorizationManager = authorizationManager;
    private readonly ISecretManager secretManager = secretManager;
    private readonly HttpContext httpContext = accessor.HttpContext!;
    private readonly Protector protector = protectorFactory.Create(ProtectorType.Secret);
    private readonly SignInOptions options = options.Value;

    public override async ValueTask<Result> SignInAsync(Dictionary<string, object> credentials, 
        CancellationToken cancellationToken = default)
    {
        SignInResponse? response;
        
        var userId = credentials["UserId"].ToString();
        var code = credentials["Code"].ToString();

        if (string.IsNullOrEmpty(code)) return Domain.Common.API.Results.BadRequest("Code is empty");
        if (string.IsNullOrEmpty(userId)) return Domain.Common.API.Results.BadRequest("User ID is empty");

        var user = await userManager.FindByIdAsync(Guid.Parse(userId), cancellationToken);
        if (user is null) return Domain.Common.API.Results.NotFound($"Cannot find user with ID {userId}.");

        var userAgent = httpContext.GetUserAgent()!;
        var ipAddress = httpContext.GetIpV4()!;
        var clientInfo = httpContext.GetClientInfo()!;
        var device = user.GetDevice(userAgent, ipAddress);

        if (device is null)
        {
            device = new UserDeviceEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                UserAgent = userAgent,
                IpAddress = ipAddress,
                Browser = clientInfo.UA.ToString(),
                OS = clientInfo.OS.ToString(),
                Device = clientInfo.Device.ToString(),
                IsTrusted = false,
                IsBlocked = false,
                FirstSeen = DateTimeOffset.UtcNow,
                CreateDate = DateTimeOffset.UtcNow
            };

            var result = await deviceManager.CreateAsync(device, cancellationToken);
            if (!result.Succeeded) return result;
        }
        
        var userSecret = await secretManager.FindAsync(user, cancellationToken);
        if (userSecret is null) return Domain.Common.API.Results.NotFound("Not found user secret");

        var unprotectedSecret = protector.Unprotect(userSecret.Secret);
        var isCodeVerified = AuthenticatorUtils.VerifyCode(code, unprotectedSecret);

        if (!isCodeVerified)
        {
            user.FailedLoginAttempts += 1;

            if (user.FailedLoginAttempts < options.MaxFailedLoginAttempts)
            {
                response = new SignInResponse()
                {
                    UserId = user.Id,
                    FailedLoginAttempts = user.FailedLoginAttempts,
                    MaxFailedLoginAttempts = options.MaxFailedLoginAttempts,
                };

                return Domain.Common.API.Results.BadRequest($"Invalid two-factor code {code}.", response);
            }

            var deviceBlockResult = await deviceManager.BlockAsync(device, cancellationToken);
            if (!deviceBlockResult.Succeeded) return deviceBlockResult;

            var lockoutResult = await lockoutManager.BlockPermanentlyAsync(user, 
                LockoutType.TooManyFailedLoginAttempts, cancellationToken: cancellationToken);

            if (!lockoutResult.Succeeded) return lockoutResult;

            response = new SignInResponse()
            {
                UserId = user.Id,
                IsLockedOut = true,
                FailedLoginAttempts = user.FailedLoginAttempts,
                MaxFailedLoginAttempts = options.MaxFailedLoginAttempts,
                Type = LockoutType.TooManyFailedLoginAttempts
            };

            return Domain.Common.API.Results.BadRequest("Account is locked out due to too many failed login attempts", response);
        }

        if (user.FailedLoginAttempts > 0)
        {
            user.FailedLoginAttempts = 0;

            var userUpdateResult = await userManager.UpdateAsync(user, cancellationToken);
            if (!userUpdateResult.Succeeded) return userUpdateResult;
        }

        response = new SignInResponse() { UserId = user.Id, };

        const string method = nameof(TwoFactorMethod.AuthenticatorApp);
        await loginManager.CreateAsync(device, LoginType.TwoFactor, method, cancellationToken);
        await authorizationManager.CreateAsync(device, cancellationToken);

        return Result.Success(response);
    }
}