using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authentication.Session;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Http.Results;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Server.Security.Authentication.SignIn.Strategies;

public sealed class OAuthSignInStrategy(
    IUserManager userManager,
    IDeviceManager deviceManager,
    ILockoutManager lockoutManager,
    IHttpContextAccessor httpContextAccessor,
    ILinkedAccountManager linkedAccountManager,
    ITwoFactorManager twoFactorManager,
    ISessionManager sessionManager,
    IPasskeyManager passkeyManager,
    IAuthenticationSessionManager authenticationSessionManager,
    IOptions<SessionOptions> options) : ISignInStrategy
{
    private readonly IUserManager _userManager = userManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly ILockoutManager _lockoutManager = lockoutManager;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly IAuthenticationSessionManager _authenticationSessionManager = authenticationSessionManager;
    private readonly SessionOptions _options = options.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async ValueTask<Result> ExecuteAsync(SignInPayload payload,
        CancellationToken cancellationToken = default)
    {
        if (payload is not OAuthSignInPayload oauthPayload)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidPayloadType,
                Description = "Invalid payload"
            });
        }

        var authenticationSession = await _authenticationSessionManager.FindByIdAsync(oauthPayload.Sid, cancellationToken);
        if (authenticationSession is null)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidSession,
                Description = "Invalid session"
            });
        }

        var user = await _userManager.FindByEmailAsync(oauthPayload.Email, cancellationToken);
        if (user is null) return Results.BadRequest("User not found.");

        var userAgent = _httpContext.GetUserAgent();
        var ipAddress = _httpContext.GetIpV4();
        var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null)
        {
            var clientInfo = _httpContext.GetClientInfo();
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
                FirstSeen = DateTimeOffset.UtcNow
            };

            var result = await _deviceManager.CreateAsync(device, cancellationToken);
            if (!result.Succeeded) return result;
        }

        if (device.IsBlocked)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.BlockedDevice,
                Description = "Device is blocked"
            });
        }

        var lockoutState = await _lockoutManager.GetAsync(user, cancellationToken);
        if (lockoutState is null) return Results.NotFound("State not found");
        
        var linkedAccount = await _linkedAccountManager.GetAsync(user, oauthPayload.Provider, cancellationToken);
        if (linkedAccount is null)
        {
            linkedAccount = new UserLinkedAccountEntity
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                Type = oauthPayload.Provider
            };
            
            var connectResult = await _linkedAccountManager.CreateAsync(linkedAccount, cancellationToken);
            if (!connectResult.Succeeded) return connectResult;
        }
        
        authenticationSession.OAuthFlow = OAuthFlow.SignIn;
        authenticationSession.UserId = user.Id;
        if (await _twoFactorManager.IsEnabledAsync(user, cancellationToken))
        {
            var hasPasskey = await _passkeyManager.HasAsync(user, cancellationToken);
            string[] allowedMfaMethods = hasPasskey
                ? [AuthenticationMethods.SoftwareKey, AuthenticationMethods.OneTimePassword]
                : [AuthenticationMethods.OneTimePassword];
            
            authenticationSession.RequiredAuthenticationMethods = [AuthenticationMethods.MultiFactorAuthentication];
            authenticationSession.AllowedMfaMethods = allowedMfaMethods;
            
            var sessionResult = await _authenticationSessionManager.UpdateAsync(authenticationSession, cancellationToken);
            if (!sessionResult.Succeeded) return sessionResult;
            
            return Results.Found(QueryBuilder.Create()
                .WithUri(oauthPayload.ReturnUri)
                .WithQueryParam("sid", authenticationSession.Id.ToString())
                .WithQueryParam("state", oauthPayload.State)
                .Build());
        }
        else
        {
            var session = new SessionEntity
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                AuthenticationMethods = authenticationSession.PassedAuthenticationMethods,
                ExpireDate = DateTimeOffset.UtcNow.Add(_options.Timestamp)
            };
            
            await _sessionManager.CreateAsync(session, cancellationToken);
            
            authenticationSession.SessionId = session.Id;
            
            var sessionResult = await _authenticationSessionManager.UpdateAsync(authenticationSession, cancellationToken);
            if (!sessionResult.Succeeded) return sessionResult;
            
            return Results.Found(QueryBuilder.Create()
                .WithUri(oauthPayload.ReturnUri)
                .WithQueryParam("sid", authenticationSession.Id.ToString())
                .WithQueryParam("state", oauthPayload.State)
                .Build());
        }
    }
}