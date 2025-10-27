using eSystem.Auth.Api.Security.Authentication.SSO.Session;
using eSystem.Auth.Api.Security.Authentication.TwoFactor.Authenticator;
using eSystem.Auth.Api.Security.Cryptography.Protection;
using eSystem.Auth.Api.Security.Identity.Options;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Responses.Auth;
using eSystem.Core.Security.Cryptography.Protection;
using eSystem.Core.Security.Lockout;

namespace eSystem.Auth.Api.Security.Authentication.SignIn.Strategies;

public class AuthenticatorSignInStrategy(
    IUserManager userManager,
    ILockoutManager lockoutManager,
    IDeviceManager deviceManager,
    ISessionManager sessionManager,
    ISecretManager secretManager,
    IHttpContextAccessor accessor,
    IProtectorFactory protectorFactory,
    IOptions<SignInOptions> options) : SignInStrategy
{
    private readonly IUserManager userManager = userManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly ISessionManager sessionManager = sessionManager;
    private readonly ISecretManager secretManager = secretManager;
    private readonly HttpContext httpContext = accessor.HttpContext!;
    private readonly SignInOptions options = options.Value;
    private readonly IProtector protector = protectorFactory.Create(ProtectionPurposes.Secret);

    public override async ValueTask<Result> SignInAsync(Dictionary<string, object> credentials, 
        CancellationToken cancellationToken = default)
    {
        SignInResponse? response;
        
        var userId = credentials["UserId"].ToString();
        var code = credentials["Code"].ToString();

        if (string.IsNullOrEmpty(code)) return eSystem.Core.Common.Results.Results.BadRequest("Code is empty");
        if (string.IsNullOrEmpty(userId)) return eSystem.Core.Common.Results.Results.BadRequest("User ID is empty");

        var user = await userManager.FindByIdAsync(Guid.Parse(userId), cancellationToken);
        if (user is null) return eSystem.Core.Common.Results.Results.NotFound($"Cannot find user with ID {userId}.");

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
        if (userSecret is null) return eSystem.Core.Common.Results.Results.NotFound("Not found user secret");

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

                return eSystem.Core.Common.Results.Results.BadRequest($"Invalid two-factor code {code}.", response);
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

            return eSystem.Core.Common.Results.Results.BadRequest("Account is locked out due to too many failed login attempts", response);
        }

        if (user.FailedLoginAttempts > 0)
        {
            user.FailedLoginAttempts = 0;

            var userUpdateResult = await userManager.UpdateAsync(user, cancellationToken);
            if (!userUpdateResult.Succeeded) return userUpdateResult;
        }

        response = new SignInResponse() { UserId = user.Id, };
        
        await sessionManager.CreateAsync(device, cancellationToken);

        return Result.Success(response);
    }
}