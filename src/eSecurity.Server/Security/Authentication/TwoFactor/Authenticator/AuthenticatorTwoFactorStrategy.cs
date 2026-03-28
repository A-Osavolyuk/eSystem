using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authentication.Lockout;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authentication.Session;
using eSecurity.Server.Security.Authentication.TwoFactor.Secret;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Cryptography.Protection.Constants;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Enums;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Primitives.Constants;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Security.Authentication.TwoFactor.Authenticator;

public sealed class AuthenticatorTwoFactorContext : TwoFactorContext
{
    public required string Code { get; set; }
}

public sealed class AuthenticatorTwoFactorStrategy(
    IAuthenticationSessionManager authenticationSessionManager,
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor,
    IDeviceManager deviceManager,
    ILockoutManager lockoutManager,
    ISessionManager sessionManager,
    ISecretManager secretManager,
    IDataProtectionProvider protectionProvider,
    IOptions<SessionOptions> sessionOptions,
    IOptions<SignInOptions> signInOptions) : ITwoFactorStrategy<AuthenticatorTwoFactorContext>
{
    private readonly IAuthenticationSessionManager _authenticationSessionManager = authenticationSessionManager;
    private readonly IUserManager _userManager = userManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly ILockoutManager _lockoutManager = lockoutManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly ISecretManager _secretManager = secretManager;
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;
    private readonly SignInOptions _signInOptions = signInOptions.Value;
    private readonly SessionOptions _sessionOptions = sessionOptions.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async ValueTask<Result> ExecuteAsync(AuthenticatorTwoFactorContext context,
        CancellationToken cancellationToken = default)
    {
        var authenticationSession = await _authenticationSessionManager.FindByIdAsync(
            context.TransactionId, cancellationToken);

        if (authenticationSession?.UserId is null ||
            authenticationSession.GetMethods(AuthenticationMethodType.AllowedMfa).Count == 0)
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

        var secret = await _secretManager.GetAsync(user, cancellationToken);
        if (secret is null) return Results.NotFound("Secret not found.");

        var protector = _protectionProvider.CreateProtector(ProtectionPurposes.Secret);
        var unprotectedSecret = protector.Unprotect(secret.ProtectedSecret);

        if (!AuthenticatorUtils.VerifyCode(context.Code, unprotectedSecret))
        {
            user.FailedLoginAttempts += 1;
            if (user.FailedLoginAttempts < _signInOptions.MaxFailedLoginAttempts)
                return Results.BadRequest(new Error
                {
                    Code = ErrorTypes.Common.FailedLoginAttempt,
                    Description = "Invalid code.",
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

        AuthenticationMethod[] authenticationMethods =
        [
            ..authenticationSession.AuthenticationMethods.Select(x => x.Method),
            AuthenticationMethod.MultiFactorAuthentication,
            AuthenticationMethod.OneTimePassword
        ];

        var session = new SessionEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            ExpireDate = DateTimeOffset.UtcNow.Add(_sessionOptions.Timestamp)
        };

        session.AddMethods(authenticationSession.AuthenticationMethods.Select(x => x.Method));
        await _sessionManager.CreateAsync(session, cancellationToken);

        authenticationSession.SessionId = session.Id;
        authenticationSession.Pass(authenticationMethods);

        var sessionResult = await _authenticationSessionManager.UpdateAsync(authenticationSession, cancellationToken);
        if (!sessionResult.Succeeded) return sessionResult;

        return Results.Ok(new SignInResponse
        {
            TransactionId = authenticationSession.Id,
            SessionId = session.Id
        });
    }
}