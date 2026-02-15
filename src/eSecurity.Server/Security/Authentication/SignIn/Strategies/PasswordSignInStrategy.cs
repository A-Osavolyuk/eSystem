using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authentication.Lockout;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Authentication.Session;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Server.Security.Authentication.SignIn.Strategies;

public sealed class PasswordSignInStrategy(
    IUserManager userManager,
    IPasswordManager passwordManager,
    ILockoutManager lockoutManager,
    IDeviceManager deviceManager,
    ISessionManager sessionManager,
    IHttpContextAccessor accessor,
    IEmailManager emailManager,
    ITwoFactorManager twoFactorManager,
    IAuthenticationSessionManager authenticationSessionManager,
    IPasskeyManager passkeyManager,
    IOptions<SignInOptions> signInOptions,
    IOptions<SessionOptions> sessionOptions) : ISignInStrategy
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly ILockoutManager _lockoutManager = lockoutManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly IAuthenticationSessionManager _authenticationSessionManager = authenticationSessionManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly SessionOptions _sessionOptions = sessionOptions.Value;
    private readonly HttpContext _httpContext = accessor.HttpContext!;
    private readonly SignInOptions _signInOptions = signInOptions.Value;

    public async ValueTask<Result> ExecuteAsync(SignInPayload payload,
        CancellationToken cancellationToken = default)
    {
        UserEntity? user = null;

        if (payload is not PasswordSignInPayload passwordPayload)
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidPayloadType,
                Description = "Invalid payload type"
            });

        if (_signInOptions.AllowUserNameLogin)
        {
            user = await _userManager.FindByUsernameAsync(passwordPayload.Login, cancellationToken);
        }

        if (user is null && _signInOptions.AllowEmailLogin)
        {
            user = await _userManager.FindByEmailAsync(passwordPayload.Login, cancellationToken);
        }

        if (user is null) return Results.NotFound($"Cannot find user with login {passwordPayload.Login}.");
        if (!await _passwordManager.HasAsync(user, cancellationToken))
            return Results.BadRequest("Cannot log in, you don't have a password.");

        var userAgent = _httpContext.GetUserAgent()!;
        var ipAddress = _httpContext.GetIpV4()!;
        var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null)
        {
            var clientInfo = _httpContext.GetClientInfo()!;
            device = new UserDeviceEntity
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                UserAgent = userAgent,
                IpAddress = ipAddress,
                Browser = clientInfo.UA.ToString(),
                Os = clientInfo.OS.ToString(),
                Device = clientInfo.Device.ToString(),
                IsBlocked = false,
                FirstSeen = DateTimeOffset.UtcNow,
            };

            var result = await _deviceManager.CreateAsync(device, cancellationToken);
            if (!result.Succeeded) return result;
        }

        var email = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
        if (email is null) return Results.NotFound("Email not found");

        if (_signInOptions.RequireConfirmedEmail && !email.IsVerified)
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.UnverifiedEmail,
                Description = "Email is not verified.",
                Details = new() { { "userId", user.Id } }
            });

        var lockoutState = await _lockoutManager.GetAsync(user, cancellationToken);
        if (lockoutState is null) return Results.NotFound("State not found");

        if (lockoutState.Enabled)
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.AccountLockedOut,
                Description = "Account is locked out",
                Details = new() { { "userId", user.Id } }
            });

        if (!await _passwordManager.HasAsync(user, cancellationToken))
            return Results.BadRequest("User doesn't have a password.");

        if (!await _passwordManager.CheckAsync(user, passwordPayload.Password, cancellationToken))
        {
            user.FailedLoginAttempts += 1;

            var updateResult = await _userManager.UpdateAsync(user, cancellationToken);
            if (!updateResult.Succeeded) return updateResult;

            if (user.FailedLoginAttempts < _signInOptions.MaxFailedLoginAttempts)
                return Results.BadRequest(new Error
                {
                    Code = ErrorTypes.Common.FailedLoginAttempt,
                    Description = "The password is not valid.",
                    Details = new()
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
                Code = ErrorTypes.Common.TooManyFailedLoginAttempts,
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

        if (device.IsBlocked)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.BlockedDevice,
                Description = "Cannot sign in, device is blocked."
            });
        }
        
        var authenticationSession = new AuthenticationSessionEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            PassedAuthenticationMethods = [AuthenticationMethods.Password],
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(15),
        };

        if (await _twoFactorManager.IsEnabledAsync(user, cancellationToken))
        {
            var hasPasskey = await _passkeyManager.HasAsync(user, cancellationToken);
            string[] allowedMfaMethods = hasPasskey
                ? [AuthenticationMethods.SoftwareKey, AuthenticationMethods.OneTimePassword]
                : [AuthenticationMethods.OneTimePassword];
            
            authenticationSession.RequiredAuthenticationMethods = [AuthenticationMethods.MultiFactorAuthentication];
            authenticationSession.AllowedMfaMethods = allowedMfaMethods;
            
            var sessionResult = await _authenticationSessionManager.CreateAsync(authenticationSession, cancellationToken);
            if (!sessionResult.Succeeded) return sessionResult;
            
            return Results.Ok(new SignInResponse()
            {
                TransactionId = authenticationSession.Id
            });
        }
        else
        {
            var session = new SessionEntity
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                AuthenticationMethods = [AuthenticationMethods.Password],
                ExpireDate = DateTimeOffset.UtcNow.Add(_sessionOptions.Timestamp)
            };

            await _sessionManager.CreateAsync(session, cancellationToken);
            
            authenticationSession.SessionId = session.Id;
            
            var sessionResult = await _authenticationSessionManager.CreateAsync(authenticationSession, cancellationToken);
            if (!sessionResult.Succeeded) return sessionResult;
            
            return Results.Ok(new SignInResponse
            {
                TransactionId = authenticationSession.Id,
                SessionId = session.Id
            });
        }
    }
}