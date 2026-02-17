using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authentication.Lockout;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authentication.Session;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Server.Security.Authentication.TwoFactor.RecoveryCode;

public sealed class RecoveryCodeTwoFactorContext : TwoFactorContext
{
    public required string Code { get; set; }
}

public sealed class RecoveryCodeTwoFactorStrategy(
    IAuthenticationSessionManager authenticationSessionManager,
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor,
    IDeviceManager deviceManager,
    ILockoutManager lockoutManager,
    ISessionManager sessionManager,
    IRecoverManager recoverManager,
    IOptions<SessionOptions> sessionOptions,
    IOptions<SignInOptions> signInOptions) : ITwoFactorStrategy<RecoveryCodeTwoFactorContext>
{
    private readonly IAuthenticationSessionManager _authenticationSessionManager = authenticationSessionManager;
    private readonly IUserManager _userManager = userManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly ILockoutManager _lockoutManager = lockoutManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IRecoverManager _recoverManager = recoverManager;
    private readonly SessionOptions _sessionOptions = sessionOptions.Value;
    private readonly SignInOptions _signInOptions = signInOptions.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async ValueTask<Result> ExecuteAsync(RecoveryCodeTwoFactorContext context,
        CancellationToken cancellationToken = default)
    {
        var authenticationSession = await _authenticationSessionManager.FindByIdAsync(
            context.TransactionId, cancellationToken);

        if (authenticationSession?.UserId is null || authenticationSession.AllowedMfaMethods is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.Common.InvalidSession,
                Description = "Invalid session"
            });
        }

        var user = await _userManager.FindByIdAsync(authenticationSession.UserId.Value, cancellationToken);
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

        var codeResult = await _recoverManager.VerifyAsync(user, context.Code, cancellationToken);
        if (!codeResult.Succeeded)
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
            ..authenticationSession.PassedAuthenticationMethods,
            AuthenticationMethods.MultiFactorAuthentication,
            AuthenticationMethods.OneTimePassword
        ];

        var session = new SessionEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            AuthenticationMethods = authenticationMethods,
            ExpireDate = DateTimeOffset.UtcNow.Add(_sessionOptions.Timestamp),
        };

        await _sessionManager.CreateAsync(session, cancellationToken);

        authenticationSession.SessionId = session.Id;
        authenticationSession.PassedAuthenticationMethods = authenticationMethods;
        authenticationSession.RequiredAuthenticationMethods = [];

        var sessionResult = await _authenticationSessionManager.UpdateAsync(authenticationSession, cancellationToken);
        if (!sessionResult.Succeeded) return sessionResult;

        return Results.Ok(new SignInResponse
        {
            TransactionId = authenticationSession.Id,
            SessionId = session.Id
        });
    }
}