using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Lockout;
using eSecurity.Idp.Security.Authentication.Session;
using eSecurity.Idp.Security.Authorization.Devices;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Credentials.PublicKey.Credentials;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Responses;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Idp.Security.Authentication.AuthenticationSession;
using eSecurity.Idp.Security.Cookies;
using eSecurity.Idp.Security.Credentials.PublicKey.Challenge;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using Session_SessionOptions = eSecurity.Idp.Security.Authentication.Session.SessionOptions;

namespace eSecurity.Idp.Security.Authentication.SignIn.Strategies;

public sealed class SoftwareKeySignInStrategy(
    IUserQueryService userQueryService,
    ILockoutManager lockoutManager,
    IHttpContextAccessor accessor,
    IAuthenticationSessionManager authenticationSessionManager,
    IOptions<Session_SessionOptions> options,
    ISoftwareKeyQueryService softwareKeyQueryService,
    ISoftwareKeyCommandService softwareKeyCommandService,
    ISessionCommandService sessionCommandService,
    IDeviceQueryService deviceQueryService,
    ISessionCookieFactory sessionCookieFactory) : SignInStrategy<SoftwareKeySignInPayload>
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly ILockoutManager _lockoutManager = lockoutManager;
    private readonly IAuthenticationSessionManager _authenticationSessionManager = authenticationSessionManager;
    private readonly ISoftwareKeyQueryService _softwareKeyQueryService = softwareKeyQueryService;
    private readonly ISoftwareKeyCommandService _softwareKeyCommandService = softwareKeyCommandService;
    private readonly ISessionCommandService _sessionCommandService = sessionCommandService;
    private readonly IDeviceQueryService _deviceQueryService = deviceQueryService;
    private readonly ISessionCookieFactory _sessionCookieFactory = sessionCookieFactory;
    private readonly Session_SessionOptions _options = options.Value;
    private readonly HttpContext _httpContext = accessor.HttpContext!;

    public override Type PayloadType => typeof(SoftwareKeySignInPayload);

    public override async ValueTask<Result> ExecuteAsync(SoftwareKeySignInPayload payload, 
        CancellationToken cancellationToken = default)
    {
        if (payload.Credential is null)
            throw new ValidationException("Credential is required");

        var credential = payload.Credential;
        var passkey = await _softwareKeyQueryService.GetByCredentialIdAsync(credential.Id, cancellationToken);
        if (passkey is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid credential"
            });
        }

        var user = await _userQueryService.GetByIdAsync(passkey.Device.UserId, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = $"Cannot find user with ID {passkey.Device.UserId}."
            });
        }

        var savedChallenge = _httpContext.Session.GetString(ChallengeSessionKeys.Assertion);
        if (string.IsNullOrEmpty(savedChallenge))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid challenge"
            });
        }

        var result = await _softwareKeyCommandService.VerifyAsync(passkey, 
            credential, savedChallenge, cancellationToken);
        
        if (!result.Succeeded) 
            return result;

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
                Details = new() { { "userId", user.Id } }
            });
        }

        var userAgent = _httpContext.GetUserAgent();
        var ipAddress = _httpContext.GetIpV4();
        var device = await _deviceQueryService.GetByMetadataAsync(user.Id, userAgent, ipAddress, cancellationToken);
        
        if (device is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.InvalidDevice, 
                Description = "Invalid device."
            });
        }

        var session = new SessionEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            ExpireDate = DateTimeOffset.UtcNow.Add(_options.Timestamp)
        };
        
        session.AddMethods(AuthenticationMethodReference.SoftwareKey);
        await _sessionCommandService.CreateAsync(session, cancellationToken);
        
        var authenticationSession = new AuthenticationSessionEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            SessionId = session.Id,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(15)
        };
        
        authenticationSession.Pass(AuthenticationMethodReference.SoftwareKey);
        
        var sessionResult = await _authenticationSessionManager.CreateAsync(authenticationSession, cancellationToken);
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
            TransactionId = authenticationSession.Id
        });
    }
}