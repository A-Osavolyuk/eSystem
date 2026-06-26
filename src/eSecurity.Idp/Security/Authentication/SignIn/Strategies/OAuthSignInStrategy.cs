using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Lockout;
using eSecurity.Idp.Security.Authentication.Session;
using eSecurity.Idp.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Authorization.Devices;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Idp.Security.Authorization.LinkedAccount;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Idp.Security.Authentication.SignIn.Strategies;

public sealed class OAuthSignInStrategy(
    IUserQueryService userQueryService,
    IDeviceManager deviceManager,
    ILockoutManager lockoutManager,
    IHttpContextAccessor httpContextAccessor,
    ILinkedAccountManager linkedAccountManager,
    IAuthenticationSessionManager authenticationSessionManager,
    ISoftwareKeyQueryService softwareKeyQueryService,
    ITwoFactorQueryService twoFactorQueryService,
    ISessionCommandService sessionCommandService,
    IOptions<SessionOptions> options) : ISignInStrategy
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly ILockoutManager _lockoutManager = lockoutManager;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;
    private readonly IAuthenticationSessionManager _authenticationSessionManager = authenticationSessionManager;
    private readonly ISoftwareKeyQueryService _softwareKeyQueryService = softwareKeyQueryService;
    private readonly ITwoFactorQueryService _twoFactorQueryService = twoFactorQueryService;
    private readonly ISessionCommandService _sessionCommandService = sessionCommandService;
    private readonly SessionOptions _options = options.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async ValueTask<Result> ExecuteAsync(SignInPayload payload,
        CancellationToken cancellationToken = default)
    {
        if (payload is not OAuthSignInPayload oauthPayload)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidPayloadType,
                Description = "Invalid payload"
            });
        }

        var authenticationSession = await _authenticationSessionManager.FindByIdAsync(oauthPayload.Sid, cancellationToken);
        if (authenticationSession is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidSession,
                Description = "Invalid session"
            });
        }

        var user = await _userQueryService.GetByEmailAsync(oauthPayload.Email, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotAcceptable, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "User not found."
            });
        }

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
                FirstSeenAt = DateTimeOffset.UtcNow
            };

            var result = await _deviceManager.CreateAsync(device, cancellationToken);
            if (!result.Succeeded) return result;
        }

        if (device.IsBlocked)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BlockedDevice,
                Description = "Device is blocked"
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

        var twoFactorMethods = await _twoFactorQueryService.ListByUserAsync(user.Id, cancellationToken);
        if (twoFactorMethods.Count > 0)
        {
            var softwareKeys = await _softwareKeyQueryService.ListByUserAsync(user.Id, cancellationToken);
            authenticationSession.AllowMfa(softwareKeys.Count > 0
                ? [AuthenticationMethodReference.SoftwareKey, AuthenticationMethodReference.OneTimePassword]
                : [AuthenticationMethodReference.OneTimePassword]);
            
            var sessionResult = await _authenticationSessionManager.UpdateAsync(authenticationSession, cancellationToken);
            if (!sessionResult.Succeeded) return sessionResult;
        }
        else
        {
            var session = new SessionEntity
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                ExpireDate = DateTimeOffset.UtcNow.Add(_options.Timestamp),
            };
            
            session.AddMethods(authenticationSession.AuthenticationMethods.Select(x => x.MethodReference));
            await _sessionCommandService.CreateAsync(session, cancellationToken);
            
            authenticationSession.SessionId = session.Id;
            var sessionResult = await _authenticationSessionManager.UpdateAsync(authenticationSession, cancellationToken);
            if (!sessionResult.Succeeded) return sessionResult;
        }

        return Results.Redirect(RedirectionCode.Found, QueryBuilder.Create()
            .WithUri(oauthPayload.ReturnUri)
            .WithQueryParam("sid", authenticationSession.Id.ToString())
            .WithQueryParam("state", oauthPayload.State)
            .Build());
    }
}