using System.Text.Json;
using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authentication.Lockout;
using eSecurity.Core.Security.Credentials.PublicKey;
using eSecurity.Core.Security.Credentials.PublicKey.Constants;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authentication.Session;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Credentials.PublicKey.Credentials;
using eSecurity.Server.Security.Cryptography.Protection.Constants;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Security.Authentication.TwoFactor.Passkey;

public sealed class PasskeyTwoFactorContext : TwoFactorContext
{
    public required PublicKeyCredential Credential { get; set; }
}

public sealed class PasskeyTwoFactorStrategy(
    IAuthenticationSessionManager authenticationSessionManager,
    IUserManager userManager,
    IHttpContextAccessor httpContextAccessor,
    IDeviceManager deviceManager,
    ILockoutManager lockoutManager,
    ISessionManager sessionManager,
    IPasskeyManager passkeyManager,
    IOptions<SessionOptions> sessionOptions,
    IOptions<SignInOptions> signInOptions,
    IDataProtectionProvider protectionProvider) : ITwoFactorStrategy<PasskeyTwoFactorContext>
{
    private readonly IAuthenticationSessionManager _authenticationSessionManager = authenticationSessionManager;
    private readonly IUserManager _userManager = userManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly ILockoutManager _lockoutManager = lockoutManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;
    private readonly SessionOptions _sessionOptions = sessionOptions.Value;
    private readonly SignInOptions _signInOptions = signInOptions.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async ValueTask<Result> ExecuteAsync(PasskeyTwoFactorContext context,
        CancellationToken cancellationToken = default)
    {
        var authenticationSession = await _authenticationSessionManager.FindByIdAsync(
            context.TransactionId, cancellationToken);

        if (authenticationSession?.UserId is null || 
            authenticationSession.GetMethods(AuthenticationMethodType.AllowedMfa).Count == 0)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidSession,
                Description = "Invalid session"
            });
        }

        var user = await _userManager.FindByIdAsync(authenticationSession.UserId.Value, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "User not found."
            });
        }

        var userAgent = _httpContext.GetUserAgent()!;
        var ipAddress = _httpContext.GetIpV4()!;
        var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null || device.IsBlocked)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidDevice,
                Description = "Invalid device."
            });
        }
        
        var credential = context.Credential;
        var credentialId = CredentialUtils.ToBase64String(credential.Id);

        var passkey = await _passkeyManager.FindByCredentialIdAsync(credentialId, cancellationToken);
        if (passkey is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidCredentials,
                Description = "Invalid credential"
            });
        }

        var savedChallenge = _httpContext.Session.GetString(ChallengeSessionKeys.Assertion);
        if (string.IsNullOrEmpty(savedChallenge))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidChallenge,
                Description = "Invalid challenge"
            });
        }

        var verificationResult = await _passkeyManager.VerifyAsync(passkey, credential, savedChallenge, cancellationToken);
        if (!verificationResult.Succeeded)
        {
            user.FailedLoginAttempts += 1;
            if (user.FailedLoginAttempts < _signInOptions.MaxFailedLoginAttempts)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.FailedLoginAttempt,
                    Description = "Invalid passkey.",
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

            if (!lockoutResult.Succeeded) 
                return lockoutResult;
            
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.AccountLockedOut,
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

        AuthenticationMethodReference[] authenticationMethods =
        [
            ..authenticationSession.AuthenticationMethods.Select(x => x.MethodReference),
            AuthenticationMethodReference.SoftwareKey
        ];

        var session = new SessionEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            ExpireDate = DateTimeOffset.UtcNow.Add(_sessionOptions.Timestamp)
        };

        session.AddMethods(authenticationMethods);
        await _sessionManager.CreateAsync(session, cancellationToken);

        authenticationSession.SessionId = session.Id;
        authenticationSession.Pass(authenticationMethods);

        var sessionResult = await _authenticationSessionManager.UpdateAsync(authenticationSession, cancellationToken);
        if (!sessionResult.Succeeded) return sessionResult;
        
        var sessionCookie = new SessionCookie() { SessionId = session.Id };
        var protector = _protectionProvider.CreateProtector(ProtectionPurposes.Session);
        var json = JsonSerializer.Serialize(sessionCookie);
        var protectedCookie = protector.Protect(json);

        return Results.Success(SuccessCodes.Ok, new SignInResponse
        {
            TransactionId = authenticationSession.Id,
            SessionCookie = protectedCookie
        });
    }
}