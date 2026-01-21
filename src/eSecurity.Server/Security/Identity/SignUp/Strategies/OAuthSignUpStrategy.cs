using eSecurity.Core.Security.Authentication.SignIn.Session;
using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Common.Messaging;
using eSecurity.Server.Common.Messaging.Messages.Email;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authentication.SignIn.Session;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Server.Security.Authorization.Permissions;
using eSecurity.Server.Security.Authorization.Roles;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Common.Messaging;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Utilities.Query;
using OAuthFlow = eSecurity.Core.Security.Authorization.OAuth.OAuthFlow;

namespace eSecurity.Server.Security.Identity.SignUp.Strategies;

public sealed class OAuthSignUpPayload : SignUpPayload
{
    public required Guid SessionId { get; set; }
    public required LinkedAccountType Type { get; set; }
    public required string Email { get; set; }
    public required string ReturnUri { get; set; }
    public required string State { get; set; }
}

public sealed class OAuthSignUpStrategy(
    IPermissionManager permissionManager,
    IUserManager userManager,
    IMessageService messageService,
    IRoleManager roleManager,
    ISignInSessionManager signInSessionManager,
    ILinkedAccountManager providerManager,
    IDeviceManager deviceManager,
    IHttpContextAccessor httpContextAccessor,
    IEmailManager emailManager,
    ISessionManager sessionManager) : ISignUpStrategy
{
    private readonly IPermissionManager _permissionManager = permissionManager;
    private readonly IUserManager _userManager = userManager;
    private readonly IMessageService _messageService = messageService;
    private readonly IRoleManager _roleManager = roleManager;
    private readonly ISignInSessionManager _signInSessionManager = signInSessionManager;
    private readonly ILinkedAccountManager _providerManager = providerManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly ISessionManager _sessionManager = sessionManager;

    public async ValueTask<Result> ExecuteAsync(SignUpPayload payload,
        CancellationToken cancellationToken = default)
    {
        if (payload is not OAuthSignUpPayload oauthPayload)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.Common.InvalidPayloadType,
                Description = "Invalid payload"
            });
        }

        var session = await _signInSessionManager.FindByIdAsync(oauthPayload.SessionId, cancellationToken);
        if (session is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.Common.NotFound,
                Description = "Session not found"
            });
        }

        var taken = await _emailManager.IsTakenAsync(oauthPayload.Email, cancellationToken);
        if (taken)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.Common.EmailTaken,
                Description = "Email is already taken"
            });
        }

        var user = new UserEntity()
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

        if (role.Permissions.Count > 0)
        {
            var permissions = role.Permissions.Select(x => x.Permission).ToList();
            foreach (var permission in permissions)
            {
                var result = await _permissionManager.GrantAsync(user, permission, cancellationToken);

                if (result.Succeeded) continue;
                return result;
            }
        }

        var userAgent = _httpContext.GetUserAgent();
        var ipAddress = _httpContext.GetIpV4();
        var clientInfo = _httpContext.GetClientInfo();

        var newDevice = new UserDeviceEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            UserAgent = userAgent,
            IpAddress = ipAddress,
            Browser = clientInfo.UA.ToString(),
            Os = clientInfo.OS.ToString(),
            Device = clientInfo.Device.ToString(),
            IsTrusted = true,
            IsBlocked = false,
            FirstSeen = DateTimeOffset.UtcNow
        };

        var deviceResult = await _deviceManager.CreateAsync(newDevice, cancellationToken);
        if (!deviceResult.Succeeded) return deviceResult;

        var email = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
        if (email is null) return Results.NotFound("Email not found");
        
        var message = new OAuthSignUpMessage()
        {
            Credentials = new Dictionary<string, string>()
            {
                { "To", email.Email },
                { "Subject", $"Account registered with {oauthPayload.Type.ToString()}" },
            },
            Payload = new()
            {
                { "UserName", user.Username },
                { "ProviderName", oauthPayload.Type.ToString() }
            }
        };

        await _messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);

        var userLinkedAccount = new UserLinkedAccountEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Type = oauthPayload.Type,
        };

        var linkedAccountResult = await _providerManager.CreateAsync(userLinkedAccount, cancellationToken);
        if (!linkedAccountResult.Succeeded) return linkedAccountResult;

        if (session.IsActive)
        {
            session.UserId = user.Id;
            session.OAuthFlow = OAuthFlow.SignUp;
            session.CompletedSteps = [SignInStep.OAuth];
            session.Status = SignInStatus.Completed;
            session.CurrentStep = SignInStep.Complete;
        }
        else
        {
            session.Status = SignInStatus.Expired;
        }
        
        var sessionResult = await _signInSessionManager.UpdateAsync(session, cancellationToken);
        if (!sessionResult.Succeeded) return sessionResult;

        await _sessionManager.CreateAsync(newDevice, cancellationToken);
        return Results.Found(QueryBuilder.Create().WithUri(oauthPayload.ReturnUri)
            .WithQueryParam("sid", session.Id.ToString())
            .WithQueryParam("state", oauthPayload.State)
            .Build());
    }
}