using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Lockout;
using eSecurity.Idp.Security.Authentication.Password;
using eSecurity.Idp.Security.Authentication.Session;
using eSecurity.Idp.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Authorization.Devices;
using eSecurity.Idp.Security.Cookies;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.Options;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Responses;
using eSecurity.Core.Security.Authentication.Lockout;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Core.Security.Identity;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Idp.Security.Authentication.SignIn.Strategies;

public sealed class PasswordSignInStrategy(
    IUserQueryService userQueryService,
    IUserFailedLoginService failedLoginService,
    IEmailQueryService emailQueryService,
    IPasswordManager passwordManager,
    ILockoutManager lockoutManager,
    IDeviceManager deviceManager,
    ISessionManager sessionManager,
    IHttpContextAccessor accessor,
    IAuthenticationSessionManager authenticationSessionManager,
    IOptions<SignInOptions> signInOptions,
    IOptions<SessionOptions> sessionOptions,
    ISoftwareKeyQueryService softwareKeyQueryService,
    ITwoFactorQueryService twoFactorQueryService,
    ISessionCookieFactory sessionCookieFactory) : ISignInStrategy
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IUserFailedLoginService _failedLoginService = failedLoginService;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly ILockoutManager _lockoutManager = lockoutManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IAuthenticationSessionManager _authenticationSessionManager = authenticationSessionManager;
    private readonly ISoftwareKeyQueryService _softwareKeyQueryService = softwareKeyQueryService;
    private readonly ITwoFactorQueryService _twoFactorQueryService = twoFactorQueryService;
    private readonly ISessionCookieFactory _sessionCookieFactory = sessionCookieFactory;
    private readonly SessionOptions _sessionOptions = sessionOptions.Value;
    private readonly HttpContext _httpContext = accessor.HttpContext!;
    private readonly SignInOptions _signInOptions = signInOptions.Value;

    public async ValueTask<Result> ExecuteAsync(SignInPayload payload,
        CancellationToken cancellationToken = default)
    {
        UserEntity? user = null;

        if (payload is not PasswordSignInPayload passwordPayload)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidPayloadType,
                Description = "Invalid payload type"
            });
        }

        if (_signInOptions.AllowUserNameLogin)
            user = await _userQueryService.GetByUsernameAsync(passwordPayload.Login, cancellationToken);

        if (user is null && _signInOptions.AllowEmailLogin)
            user = await _userQueryService.GetByEmailAsync(passwordPayload.Login, cancellationToken);

        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = $"Cannot find user with login {passwordPayload.Login}."
            });
        }

        if (!await _passwordManager.HasAsync(user, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Cannot log in, you don't have a password."
            });
        }

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
                FirstSeenAt = DateTimeOffset.UtcNow,
            };

            var result = await _deviceManager.CreateAsync(device, cancellationToken);
            if (!result.Succeeded) return result;
        }

        var email = await _emailQueryService.GetByTypeAsync(user.Id, EmailType.Primary, cancellationToken);
        if (email is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Email not found"
            });
        }

        if (_signInOptions.RequireConfirmedEmail && !email.IsVerified)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.UnverifiedEmail,
                Description = "Email is not verified.",
                Details = new Dictionary<string, object> { { "userId", user.Id } }
            });
        }

        var lockoutState = await _lockoutManager.GetAsync(user, cancellationToken);
        if (lockoutState is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "State not found"
            });
        }

        if (lockoutState.Enabled)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.AccountLockedOut,
                Description = "Account is locked out",
                Details = new Dictionary<string, object> { { "userId", user.Id } }
            });
        }

        if (!await _passwordManager.HasAsync(user, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "User doesn't have a password."
            });
        }

        if (!await _passwordManager.CheckAsync(user, passwordPayload.Password, cancellationToken))
        {
            var updateResult = await _failedLoginService.IncrementAttemptAsync(user, cancellationToken);
            if (!updateResult.Succeeded) return updateResult;

            if (user.FailedLoginAttempts < _signInOptions.MaxFailedLoginAttempts)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.FailedLoginAttempt,
                    Description = "The password is not valid.",
                    Details = new Dictionary<string, object>
                    {
                        { "maxFailedLoginAttempts", _signInOptions.MaxFailedLoginAttempts },
                        { "failedLoginAttempts", user.FailedLoginAttempts },
                    }
                });
            }

            var deviceBlockResult = await _deviceManager.BlockAsync(device, cancellationToken);
            if (!deviceBlockResult.Succeeded) return deviceBlockResult;

            var lockoutResult = await _lockoutManager.BlockPermanentlyAsync(user,
                LockoutType.TooManyFailedLoginAttempts, cancellationToken: cancellationToken);

            if (!lockoutResult.Succeeded) return lockoutResult;

            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.TooManyFailedLoginAttempts,
                Description = "Account is locked out due to too many failed login attempts",
                Details = new Dictionary<string, object> { { "userId", user.Id } }
            });
        }

        if (user.FailedLoginAttempts > 0)
        {
            var userUpdateResult = await _failedLoginService.ResetAttemptsAsync(user, cancellationToken);
            if (!userUpdateResult.Succeeded) return userUpdateResult;
        }

        if (device.IsBlocked)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BlockedDevice,
                Description = "Cannot sign in, device is blocked."
            });
        }

        var authSession = new AuthenticationSessionEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(15),
        };

        authSession.Pass(AuthenticationMethodReference.Password);

        var twoFactorMethods = await _twoFactorQueryService.ListByUserAsync(user.Id, cancellationToken);
        if (twoFactorMethods.Count > 0)
        {
            var softwareKeys = await _softwareKeyQueryService.ListByUserAsync(user.Id, cancellationToken);
            AuthenticationMethodReference[] mfaMethods = softwareKeys.Count > 0
                ? [AuthenticationMethodReference.OneTimePassword, AuthenticationMethodReference.SoftwareKey]
                : [AuthenticationMethodReference.OneTimePassword];
            
            var requiredMfaMethod = softwareKeys.Count > 0
                ? AuthenticationMethodReference.SoftwareKey
                : AuthenticationMethodReference.OneTimePassword;
                
            authSession.RequireMfa = true;
            authSession.Require(requiredMfaMethod);
            authSession.AllowMfa(mfaMethods);

            var sessionResult = await _authenticationSessionManager.CreateAsync(authSession, cancellationToken);
            if (!sessionResult.Succeeded) return sessionResult;

            var response = new SignInResponse
            {
                TransactionId = authSession.Id, 
                Require2Fa = true
            };
            
            return Results.Success(SuccessCodes.Ok, response);
        }
        else
        {
            var session = new SessionEntity
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                ExpireDate = DateTimeOffset.UtcNow.Add(_sessionOptions.Timestamp)
            };

            session.AddMethods(AuthenticationMethodReference.Password);
            await _sessionManager.CreateAsync(session, cancellationToken);

            authSession.SessionId = session.Id;

            var sessionResult = await _authenticationSessionManager.CreateAsync(authSession, cancellationToken);
            if (!sessionResult.Succeeded) return sessionResult;

            var sessionCookie = _sessionCookieFactory.CreateCookie(session);
            _httpContext.Response.Cookies.Append(DefaultCookies.Session, sessionCookie, new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(30),
            });
            
            return Results.Success(SuccessCodes.Ok, new SignInResponse
            {
                TransactionId = authSession.Id,
                Require2Fa = false
            });
        }
    }
}