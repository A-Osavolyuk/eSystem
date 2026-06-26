using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Session;
using eSecurity.Idp.Security.Authorization.Devices;
using eSecurity.Idp.Security.Authorization.Roles;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Common.Messaging.Email;
using eSecurity.Idp.Common.Messaging.Email.Builders;
using eSecurity.Idp.Security.Authentication.AuthenticationSession;
using eSecurity.Idp.Security.Authorization.LinkedAccount;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Utilities.Query;
using Session_SessionOptions = eSecurity.Idp.Security.Authentication.Session.SessionOptions;

namespace eSecurity.Idp.Security.Identity.SignUp.Strategies;

public sealed class OAuthSignUpPayload : SignUpPayload
{
    public required LinkedAccountType Provider { get; set; }
    public required string Email { get; set; }
    public required string ReturnUri { get; set; }
    public required string State { get; set; }
    public required Guid Sid { get; set; }
}

public sealed class OAuthSignUpStrategy(
    IRoleManager roleManager,
    ILinkedAccountManager providerManager,
    IDeviceManager deviceManager,
    IHttpContextAccessor httpContextAccessor,
    IAuthenticationSessionManager authenticationSessionManager,
    IOptions<Session_SessionOptions> sessionOptions,
    IEmailQueryService emailQueryService,
    IEmailCommandService emailCommandService,
    IUserCommandService userCommandService,
    ISessionCommandService sessionCommandService,
    IEmailService emailService) : ISignUpStrategy
{
    private readonly IRoleManager _roleManager = roleManager;
    private readonly ILinkedAccountManager _providerManager = providerManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly IAuthenticationSessionManager _authenticationSessionManager = authenticationSessionManager;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly IEmailCommandService _emailCommandService = emailCommandService;
    private readonly IUserCommandService _userCommandService = userCommandService;
    private readonly ISessionCommandService _sessionCommandService = sessionCommandService;
    private readonly IEmailService _emailService = emailService;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly Session_SessionOptions _sessionOptions = sessionOptions.Value;

    public async ValueTask<Result> ExecuteAsync(SignUpPayload payload,
        CancellationToken cancellationToken = default)
    {
        if (payload is not OAuthSignUpPayload oauthPayload)
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

        var taken = await _emailQueryService.ExistsAsync(oauthPayload.Email, cancellationToken);
        if (taken)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.EmailTaken,
                Description = "Email is already taken"
            });
        }

        var user = new UserEntity(oauthPayload.Email);
        var createResult = await _userCommandService.CreateAsync(user, cancellationToken: cancellationToken);
        if (!createResult.Succeeded) return createResult;

        var setResult = await _emailCommandService.AddAsync(user.Id, oauthPayload.Email, 
            EmailType.Primary, cancellationToken);
        
        if (!setResult.Succeeded) 
            return setResult;

        var role = await _roleManager.FindByNameAsync("User", cancellationToken);
        if (role is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Cannot find role 'User'"
            });
        }

        var assignResult = await _roleManager.AssignAsync(user, role, cancellationToken);
        if (!assignResult.Succeeded) return assignResult;

        var userAgent = _httpContext.GetUserAgent();
        var ipAddress = _httpContext.GetIpV4();
        var clientInfo = _httpContext.GetClientInfo();

        var newDevice = new UserDeviceEntity
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

        var deviceResult = await _deviceManager.CreateAsync(newDevice, cancellationToken);
        if (!deviceResult.Succeeded) return deviceResult;

        var email = await _emailQueryService.GetByTypeAsync(user.Id, EmailType.Primary, cancellationToken);
        if (email is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Email not found"
            });
        }

        var emailContext = new OAuthSignedUpEmailContext()
        {
            Subject = $"Sign Up with {oauthPayload.Provider.ToString()}",
            To = email.Email,
            Provider = oauthPayload.Provider.ToString()
        };

        await _emailService.SendAsync(emailContext, cancellationToken);

        var userLinkedAccount = new UserLinkedAccountEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Type = oauthPayload.Provider,
        };

        var linkedAccountResult = await _providerManager.CreateAsync(userLinkedAccount, cancellationToken);
        if (!linkedAccountResult.Succeeded) return linkedAccountResult;

        var session = new SessionEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            ExpireDate = DateTimeOffset.UtcNow.Add(_sessionOptions.Timestamp)
        };
        
        session.AddMethods(authenticationSession.AuthenticationMethods.Select(x => x.MethodReference));
        await _sessionCommandService.CreateAsync(session, cancellationToken);
        
        authenticationSession.OAuthFlow = OAuthFlow.SignUp;
        authenticationSession.UserId = user.Id;
        authenticationSession.SessionId = session.Id;
        
        var sessionResult = await _authenticationSessionManager.UpdateAsync(authenticationSession, cancellationToken);
        if (!sessionResult.Succeeded) return sessionResult;
        
        return Results.Redirect(RedirectionCode.Found, QueryBuilder.Create()
            .WithUri(oauthPayload.ReturnUri)
            .WithQueryParam("sid", authenticationSession.Id.ToString())
            .WithQueryParam("state", oauthPayload.State)
            .Build());
    }
}