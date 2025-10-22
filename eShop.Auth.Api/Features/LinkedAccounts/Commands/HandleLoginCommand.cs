using System.Security.Claims;
using eShop.Auth.Api.Messages.Email;
using eShop.Auth.Api.Security.Authentication.Results;
using eShop.Auth.Api.Security.Authentication.SignIn;
using eShop.Auth.Api.Security.Authorization.OAuth;

namespace eShop.Auth.Api.Features.LinkedAccounts.Commands;

public sealed record HandleLoginCommand(
    AuthenticationResult AuthenticationResult,
    string? RemoteError,
    string? ReturnUri) : IRequest<Result>;

public sealed class HandleOAuthLoginCommandHandler(
    IPermissionManager permissionManager,
    IUserManager userManager,
    IMessageService messageService,
    IRoleManager roleManager,
    IOAuthSessionManager sessionManager,
    ILinkedAccountManager providerManager,
    IDeviceManager deviceManager,
    ISignInResolver signInResolver,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<HandleLoginCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;
    private readonly IRoleManager roleManager = roleManager;
    private readonly IOAuthSessionManager sessionManager = sessionManager;
    private readonly ILinkedAccountManager providerManager = providerManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly ISignInResolver signInResolver = signInResolver;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(HandleLoginCommand request,
        CancellationToken cancellationToken)
    {
        var authenticationResult = request.AuthenticationResult;
        var items = authenticationResult.Properties.Items;
        var linkedAccountName = request.AuthenticationResult.Principal.Identity!.AuthenticationType!;
        var fallbackUri = items["fallbackUri"]!;
        var sessionId = items["sessionId"]!;
        var token = items["token"]!;

        var linkedAccountType = linkedAccountName switch
        {
            AuthenticationTypes.Google => LinkedAccountType.Google,
            AuthenticationTypes.Facebook => LinkedAccountType.Facebook,
            AuthenticationTypes.Microsoft => LinkedAccountType.Microsoft,
            AuthenticationTypes.X or AuthenticationTypes.Twitter => LinkedAccountType.X,
            _ => throw new NotSupportedException("Unknown linked account type")
         };

        if (!string.IsNullOrEmpty(request.RemoteError)) 
            return Results.InternalServerError(request.RemoteError, fallbackUri);

        var email = request.AuthenticationResult.Principal.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        if (email is null) return Results.BadRequest("Email is not provider in credentials", fallbackUri);
        
        var session = await sessionManager.FindAsync(Guid.Parse(sessionId), token, cancellationToken);
        if (session is null) return Results.NotFound("Session was not found", fallbackUri);

        var user = await userManager.FindByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            user = new UserEntity()
            {
                Id = Guid.CreateVersion7(),
                Username = email,
            };

            var createResult = await userManager.CreateAsync(user, cancellationToken: cancellationToken);
            if (!createResult.Succeeded) return Result.Failure(createResult.GetError(), fallbackUri);

            var setResult = await userManager.SetEmailAsync(user, email, EmailType.Primary, cancellationToken);
            if (setResult.Succeeded) return setResult;

            var role = await roleManager.FindByNameAsync("User", cancellationToken);
            if (role is null) return Results.NotFound("Cannot find role 'User'", fallbackUri);

            var assignResult = await roleManager.AssignAsync(user, role, cancellationToken);
            if (!assignResult.Succeeded) return Result.Failure(assignResult.GetError(), fallbackUri);

            if (role.Permissions.Count > 0)
            {
                var permissions = role.Permissions.Select(x => x.Permission).ToList();
                foreach (var permission in permissions)
                {
                    var result = await permissionManager.GrantAsync(user, permission, cancellationToken);

                    if (result.Succeeded) continue;
                    return Result.Failure(result.GetError(), fallbackUri);
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
            if (!deviceResult.Succeeded) return Result.Failure(deviceResult.GetError(), fallbackUri);

            var message = new OAuthSignUpMessage()
            {
                Credentials = new Dictionary<string, string>()
                {
                    { "To", user.GetEmail(EmailType.Primary)?.Email! },
                    { "Subject", $"Account registered with {linkedAccountName}" },
                },
                Payload = new()
                {
                    { "UserName", user.Username },
                    { "ProviderName", linkedAccountName }
                }
            };

            await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);

            var userLinkedAccount = new UserLinkedAccountEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                Allowed = true,
                Type = linkedAccountType,
                CreateDate = DateTimeOffset.UtcNow
            };
                
            var linkedAccountResult = await providerManager.CreateAsync(userLinkedAccount, cancellationToken);
            if(!linkedAccountResult.Succeeded) return Result.Failure(linkedAccountResult.GetError(), fallbackUri);

            session.SignType = OAuthSignType.SignUp;
            session.LinkedAccountId = userLinkedAccount.Id;

            var sessionResult = await sessionManager.UpdateAsync(session, cancellationToken);
            if (!sessionResult.Succeeded) return Result.Failure(sessionResult.GetError(), fallbackUri);
            
            var link = UrlGenerator.Url(request.ReturnUri!, new { sessionId = session.Id, token });
            return Result.Success(link);
        }

        var credentials = new Dictionary<string, object>()
        {
            { "Email", email },
            { "FallbackUri", fallbackUri },
            { "ReturnUri", request.ReturnUri! },
            { "Type", linkedAccountType },
            { "SessionId", sessionId },
            { "Token", token },
        };
        
        var strategy = signInResolver.Resolve(SignInType.LinkedAccount);
        return await strategy.SignInAsync(credentials, cancellationToken);
    }
}