using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Common.Messaging;
using eSecurity.Server.Common.Messaging.Messages.Email;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Odic.Session;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Authorization.OAuth;
using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Server.Security.Authorization.Permissions;
using eSecurity.Server.Security.Authorization.Roles;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Common.Messaging;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Server.Security.Identity.SignUp.Strategies;

public sealed class OAuthSignUpPayload : SignUpPayload
{
    public required Guid SessionId { get; set; }
    public required LinkedAccountType Type { get; set; }
    public required string Email { get; set; }
    public required string ReturnUri { get; set; }
    public required string Token { get; set; }
}

public sealed class OAuthSignUpStrategy(
    IPermissionManager permissionManager,
    IUserManager userManager,
    IMessageService messageService,
    IRoleManager roleManager,
    IOAuthSessionManager oauthSessionManager,
    ILinkedAccountManager providerManager,
    IDeviceManager deviceManager,
    IHttpContextAccessor httpContextAccessor,
    ISessionManager sessionManager) : ISignUpStrategy
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;
    private readonly IRoleManager roleManager = roleManager;
    private readonly IOAuthSessionManager oauthSessionManager = oauthSessionManager;
    private readonly ILinkedAccountManager providerManager = providerManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;
    private readonly ISessionManager sessionManager = sessionManager;

    public async ValueTask<Result> ExecuteAsync(SignUpPayload payload,
        CancellationToken cancellationToken = default)
    {
        if (payload is not OAuthSignUpPayload oauthPayload)
            return Results.BadRequest("Invalid payload");

        var session = await oauthSessionManager.FindAsync(oauthPayload.SessionId, oauthPayload.Token, cancellationToken);
        if (session is null) return Results.NotFound("Session was not found");

        var taken = await userManager.IsEmailTakenAsync(oauthPayload.Email, cancellationToken);
        if (taken) return Results.BadRequest("Email is already taken");

        var user = new UserEntity()
        {
            Id = Guid.CreateVersion7(),
            Username = oauthPayload.Email,
        };

        var createResult = await userManager.CreateAsync(user, cancellationToken: cancellationToken);
        if (!createResult.Succeeded) return Result.Failure(createResult.GetError());

        var setResult = await userManager.SetEmailAsync(user, oauthPayload.Email, EmailType.Primary, cancellationToken);
        if (!setResult.Succeeded) return setResult;

        var role = await roleManager.FindByNameAsync("User", cancellationToken);
        if (role is null) return Results.NotFound("Cannot find role 'User'");

        var assignResult = await roleManager.AssignAsync(user, role, cancellationToken);
        if (!assignResult.Succeeded) return Result.Failure(assignResult.GetError());

        if (role.Permissions.Count > 0)
        {
            var permissions = role.Permissions.Select(x => x.Permission).ToList();
            foreach (var permission in permissions)
            {
                var result = await permissionManager.GrantAsync(user, permission, cancellationToken);

                if (result.Succeeded) continue;
                return Result.Failure(result.GetError());
            }
        }

        var userAgent = httpContext.GetUserAgent();
        var ipAddress = httpContext.GetIpV4();
        var clientInfo = httpContext.GetClientInfo();

        var newDevice = new UserDeviceEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            UserAgent = userAgent,
            IpAddress = ipAddress,
            Browser = clientInfo.UA.ToString(),
            OS = clientInfo.OS.ToString(),
            Device = clientInfo.Device.ToString(),
            IsTrusted = true,
            IsBlocked = false,
            FirstSeen = DateTimeOffset.UtcNow,
            CreateDate = DateTimeOffset.UtcNow
        };

        var deviceResult = await deviceManager.CreateAsync(newDevice, cancellationToken);
        if (!deviceResult.Succeeded) return Result.Failure(deviceResult.GetError());

        var message = new OAuthSignUpMessage()
        {
            Credentials = new Dictionary<string, string>()
            {
                { "To", user.GetEmail(EmailType.Primary)?.Email! },
                { "Subject", $"Account registered with {oauthPayload.Type.ToString()}" },
            },
            Payload = new()
            {
                { "UserName", user.Username },
                { "ProviderName", oauthPayload.Type.ToString() }
            }
        };

        await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);

        var userLinkedAccount = new UserLinkedAccountEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Type = oauthPayload.Type,
            CreateDate = DateTimeOffset.UtcNow
        };

        var linkedAccountResult = await providerManager.CreateAsync(userLinkedAccount, cancellationToken);
        if (!linkedAccountResult.Succeeded)
            return Result.Failure(linkedAccountResult.GetError());

        session.SignType = OAuthSignType.SignUp;
        session.LinkedAccountId = userLinkedAccount.Id;

        var sessionResult = await oauthSessionManager.UpdateAsync(session, cancellationToken);
        if (!sessionResult.Succeeded) return Result.Failure(sessionResult.GetError());

        await sessionManager.CreateAsync(newDevice, cancellationToken);


        var builder = QueryBuilder.Create().WithUri(oauthPayload.ReturnUri)
            .WithQueryParam("sessionId", session.Id.ToString())
            .WithQueryParam("token", oauthPayload.Token);

        return Result.Success(builder.Build());
    }
}