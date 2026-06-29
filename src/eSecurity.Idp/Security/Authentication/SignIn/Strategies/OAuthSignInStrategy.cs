using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Lockout;
using eSecurity.Idp.Security.Authentication.Session;
using eSecurity.Idp.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Authorization.Devices;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Idp.Security.Authentication.AuthenticationSession;
using eSecurity.Idp.Security.Authorization.LinkedAccount;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Idp.Security.Authentication.SignIn.Strategies;

public sealed class OAuthSignInStrategy(
    IUserQueryService userQueryService,
    ILockoutManager lockoutManager,
    IHttpContextAccessor httpContextAccessor,
    IAuthenticationSessionManager authenticationSessionManager,
    ISoftwareKeyQueryService softwareKeyQueryService,
    ITwoFactorQueryService twoFactorQueryService,
    ISessionCommandService sessionCommandService,
    IDeviceQueryService deviceQueryService,
    IDeviceCommandService deviceCommandService,
    ILinkedAccountCommandService linkedAccountCommandService,
    ILinkedAccountQueryService linkedAccountQueryService,
    IOptions<SessionOptions> options) : SignInStrategy<OAuthSignInPayload>
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly ILockoutManager _lockoutManager = lockoutManager;
    private readonly IAuthenticationSessionManager _authenticationSessionManager = authenticationSessionManager;
    private readonly ISoftwareKeyQueryService _softwareKeyQueryService = softwareKeyQueryService;
    private readonly ITwoFactorQueryService _twoFactorQueryService = twoFactorQueryService;
    private readonly ISessionCommandService _sessionCommandService = sessionCommandService;
    private readonly IDeviceQueryService _deviceQueryService = deviceQueryService;
    private readonly IDeviceCommandService _deviceCommandService = deviceCommandService;
    private readonly ILinkedAccountCommandService _linkedAccountCommandService = linkedAccountCommandService;
    private readonly ILinkedAccountQueryService _linkedAccountQueryService = linkedAccountQueryService;
    private readonly SessionOptions _options = options.Value;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public override Type PayloadType => typeof(OAuthSignInPayload);

    public override async ValueTask<Result> ExecuteAsync(OAuthSignInPayload payload,
        CancellationToken cancellationToken = default)
    {
        var authenticationSession = await _authenticationSessionManager.FindByIdAsync(payload.Sid, cancellationToken);
        if (authenticationSession is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidSession,
                Description = "Invalid session"
            });
        }

        var user = await _userQueryService.GetByEmailAsync(payload.Email, cancellationToken);
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
        var device = await _deviceQueryService.GetByMetadataAsync(user.Id, userAgent, ipAddress, cancellationToken);
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

            var result = await _deviceCommandService.CreateAsync(device, cancellationToken);
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

        var linkedAccount = await _linkedAccountQueryService.GetByTypeAsync(user.Id, 
            payload.Provider, cancellationToken);
        
        if (linkedAccount is null)
        {
            var connectResult = await _linkedAccountCommandService.CreateAsync(user.Id, 
                payload.Provider, cancellationToken);
            
            if (!connectResult.Succeeded) 
                return connectResult;
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

            var sessionResult =
                await _authenticationSessionManager.UpdateAsync(authenticationSession, cancellationToken);
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
            var sessionResult =
                await _authenticationSessionManager.UpdateAsync(authenticationSession, cancellationToken);
            if (!sessionResult.Succeeded) return sessionResult;
        }

        return Results.Redirect(RedirectionCode.Found, QueryBuilder.Create()
            .WithUri(payload.ReturnUri)
            .WithQueryParam("sid", authenticationSession.Id.ToString())
            .WithQueryParam("state", payload.State)
            .Build());
    }
}