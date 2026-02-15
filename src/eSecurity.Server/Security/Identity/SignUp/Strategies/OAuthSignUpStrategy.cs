using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Common.Messaging;
using eSecurity.Server.Common.Messaging.Messages.Email;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authentication.Session;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Server.Security.Authorization.Roles;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Common.Messaging;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Server.Security.Identity.SignUp.Strategies;

public sealed class OAuthSignUpPayload : SignUpPayload
{
    public required LinkedAccountType Provider { get; set; }
    public required string Email { get; set; }
    public required string ReturnUri { get; set; }
    public required string State { get; set; }
    public required Guid Sid { get; set; }
}

public sealed class OAuthSignUpStrategy(
    IUserManager userManager,
    IMessageService messageService,
    IRoleManager roleManager,
    ILinkedAccountManager providerManager,
    IDeviceManager deviceManager,
    IHttpContextAccessor httpContextAccessor,
    IEmailManager emailManager,
    ISessionManager sessionManager,
    IAuthenticationSessionManager authenticationSessionManager,
    IOptions<SessionOptions> sessionOptions) : ISignUpStrategy
{
    private readonly IUserManager _userManager = userManager;
    private readonly IMessageService _messageService = messageService;
    private readonly IRoleManager _roleManager = roleManager;
    private readonly ILinkedAccountManager _providerManager = providerManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IAuthenticationSessionManager _authenticationSessionManager = authenticationSessionManager;
    private readonly SessionOptions _sessionOptions = sessionOptions.Value;

    public async ValueTask<Result> ExecuteAsync(SignUpPayload payload,
        CancellationToken cancellationToken = default)
    {
        if (payload is not OAuthSignUpPayload oauthPayload)
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

        var taken = await _emailManager.IsTakenAsync(oauthPayload.Email, cancellationToken);
        if (taken)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.EmailTaken,
                Description = "Email is already taken"
            });
        }

        var user = new UserEntity
        {
            Id = Guid.CreateVersion7(),
            Username = oauthPayload.Email,
        };

        var createResult = await _userManager.CreateAsync(user, cancellationToken: cancellationToken);
        if (!createResult.Succeeded) return createResult;

        var setResult = await _emailManager.SetAsync(user, oauthPayload.Email, EmailType.Primary, cancellationToken);
        if (!setResult.Succeeded) return setResult;

        var role = await _roleManager.FindByNameAsync("User", cancellationToken);
        if (role is null) return Results.NotFound("Cannot find role 'User'");

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
            FirstSeen = DateTimeOffset.UtcNow
        };

        var deviceResult = await _deviceManager.CreateAsync(newDevice, cancellationToken);
        if (!deviceResult.Succeeded) return deviceResult;

        var email = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
        if (email is null) return Results.NotFound("Email not found");
        
        var message = new EmailMessage
        {
            Credentials = new Dictionary<string, string>
            {
                { "To", email.Email },
                { "Subject", $"Sign Up with {oauthPayload.Provider.ToString()}" },
            },
            Payload = new()
            {
                { "Content", $"Your account was successfully signed-up with {oauthPayload.Provider.ToString()}" }
            }
        };

        await _messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);

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
            AuthenticationMethods = authenticationSession.PassedAuthenticationMethods,
            ExpireDate = DateTimeOffset.UtcNow.Add(_sessionOptions.Timestamp)
        };
        
        await _sessionManager.CreateAsync(session, cancellationToken);
        
        authenticationSession.OAuthFlow = OAuthFlow.SignUp;
        authenticationSession.UserId = user.Id;
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