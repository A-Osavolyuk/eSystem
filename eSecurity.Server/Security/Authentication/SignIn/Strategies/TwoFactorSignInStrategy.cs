using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authentication.Lockout;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Core.Security.Authentication.SignIn.Session;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Authentication.Oidc.Session;
using eSecurity.Server.Security.Authentication.SignIn.Session;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Http.Context;

namespace eSecurity.Server.Security.Authentication.SignIn.Strategies;

public class TwoFactorSignInStrategy(
    ISignInSessionManager signInSessionManager,
    IVerificationManager verificationManager,
    IHttpContextAccessor httpContextAccessor,
    IUserManager userManager,
    ILockoutManager lockoutManager,
    IDeviceManager deviceManager,
    ISessionManager sessionManager,
    IOptions<SignInOptions> options) : ISignInStrategy
{
    private readonly ISignInSessionManager _signInSessionManager = signInSessionManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly IUserManager _userManager = userManager;
    private readonly ILockoutManager _lockoutManager = lockoutManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly SignInOptions _options = options.Value;

    public async ValueTask<Result> ExecuteAsync(SignInPayload payload, CancellationToken cancellationToken = default)
    {
        if (payload is not TwoFactorSignInPayload twoFactorPayload)
        {
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.InvalidPayloadType,
                Description = "Invalid payload type"
            });
        }

        var session = await _signInSessionManager.FindByIdAsync(twoFactorPayload.Sid, cancellationToken);
        if (session is null || !session.IsActive)
        {
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.InvalidSession,
                Description = "Invalid sign-in session."
            });
        }

        var user = await _userManager.FindByIdAsync(session.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");
        
        var userAgent = _httpContext.GetUserAgent()!;
        var ipAddress = _httpContext.GetIpV4()!;
        var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null || device.IsBlocked)
        {
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.InvalidDevice,
                Description = "Invalid device."
            });
        }

        var verificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.TwoFactor, ActionType.SignIn, cancellationToken);

        if (!verificationResult.Succeeded)
        {
            user.FailedLoginAttempts += 1;
            if (user.FailedLoginAttempts < _options.MaxFailedLoginAttempts)
                return Results.BadRequest(new Error()
                {
                    Code = Errors.Common.FailedLoginAttempt, 
                    Description = "Invalid recovery code.",
                    Details = new()
                    {
                        { "maxFailedLoginAttempts", _options.MaxFailedLoginAttempts },
                        { "failedLoginAttempts", user.FailedLoginAttempts },
                    }
                });

            var deviceBlockResult = await _deviceManager.BlockAsync(device, cancellationToken);
            if (!deviceBlockResult.Succeeded) return deviceBlockResult;

            var lockoutResult = await _lockoutManager.BlockPermanentlyAsync(user,
                LockoutType.TooManyFailedLoginAttempts, cancellationToken: cancellationToken);

            if (!lockoutResult.Succeeded) return lockoutResult;
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.AccountLockedOut,
                Description = "Account is locked out due to too many failed login attempts",
                Details = new() { { "userId", user.Id } }
            });
        }
        
        if (user.FailedLoginAttempts > 0)
        {
            user.FailedLoginAttempts = 0;

            var userUpdateResult = await _userManager.UpdateAsync(user, cancellationToken);
            if (!userUpdateResult.Succeeded) return userUpdateResult;
        }
        
        await _sessionManager.CreateAsync(device, cancellationToken);
        return Results.Ok(new SignInResponse() { SessionId = session.Id });
    }
}