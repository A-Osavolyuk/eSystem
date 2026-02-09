using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authentication.Lockout;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Server.Security.Authentication.SignIn.Strategies;

public class TwoFactorSignInStrategy(
    IVerificationManager verificationManager,
    IHttpContextAccessor httpContextAccessor,
    IUserManager userManager,
    ILockoutManager lockoutManager,
    IDeviceManager deviceManager,
    ISessionManager sessionManager,
    IOptions<SignInOptions> signInOptions,
    IOptions<SessionOptions> sessionOptions) : ISignInStrategy
{
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly IUserManager _userManager = userManager;
    private readonly ILockoutManager _lockoutManager = lockoutManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly SignInOptions _signInOptions = signInOptions.Value;
    private readonly SessionOptions _sessionOptions = sessionOptions.Value;

    public async ValueTask<Result> ExecuteAsync(SignInPayload payload, CancellationToken cancellationToken = default)
    {
        if (payload is not TwoFactorSignInPayload twoFactorPayload)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidPayloadType,
                Description = "Invalid payload type"
            });
        }

        var user = await _userManager.FindByIdAsync(twoFactorPayload.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");
        
        var userAgent = _httpContext.GetUserAgent()!;
        var ipAddress = _httpContext.GetIpV4()!;
        var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null || device.IsBlocked)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidDevice,
                Description = "Invalid device."
            });
        }

        var verificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.TwoFactor, ActionType.SignIn, cancellationToken);

        if (!verificationResult.Succeeded)
        {
            user.FailedLoginAttempts += 1;
            if (user.FailedLoginAttempts < _signInOptions.MaxFailedLoginAttempts)
                return Results.BadRequest(new Error
                {
                    Code = ErrorTypes.Common.FailedLoginAttempt, 
                    Description = "Invalid recovery code.",
                    Details = new Dictionary<string, object>
                    {
                        { "maxFailedLoginAttempts", _signInOptions.MaxFailedLoginAttempts },
                        { "failedLoginAttempts", user.FailedLoginAttempts },
                    }
                });

            var deviceBlockResult = await _deviceManager.BlockAsync(device, cancellationToken);
            if (!deviceBlockResult.Succeeded) return deviceBlockResult;

            var lockoutResult = await _lockoutManager.BlockPermanentlyAsync(user,
                LockoutType.TooManyFailedLoginAttempts, cancellationToken: cancellationToken);

            if (!lockoutResult.Succeeded) return lockoutResult;
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.AccountLockedOut,
                Description = "Account is locked out due to too many failed login attempts",
                Details = new Dictionary<string, object> { { "userId", user.Id } }
            });
        }
        
        if (user.FailedLoginAttempts > 0)
        {
            user.FailedLoginAttempts = 0;

            var userUpdateResult = await _userManager.UpdateAsync(user, cancellationToken);
            if (!userUpdateResult.Succeeded) return userUpdateResult;
        }

        string[] authenticationMethods =
        [
            ..twoFactorPayload.AuthenticationMethods,
            AuthenticationMethods.MultiFactorAuthentication
        ];
        
        var session = new SessionEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            AuthenticationMethods = authenticationMethods,
            ExpireDate = DateTimeOffset.UtcNow.Add(_sessionOptions.Timestamp),
        };
        
        await _sessionManager.CreateAsync(session, cancellationToken);
        return Results.Ok(new SignInResponse { UserId = user.Id });
    }
}