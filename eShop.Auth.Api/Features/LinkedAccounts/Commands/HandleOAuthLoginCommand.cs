using System.Security.Claims;
using eShop.Auth.Api.Messages.Email;
using eShop.Auth.Api.Security.Authentication;

namespace eShop.Auth.Api.Features.LinkedAccounts.Commands;

public sealed record HandleOAuthLoginCommand(
    AuthenticationResult AuthenticationResult,
    string? RemoteError,
    string? ReturnUri) : IRequest<Result>;

public sealed class HandleOAuthLoginCommandHandler(
    IPermissionManager permissionManager,
    IUserManager userManager,
    IMessageService messageService,
    IRoleManager roleManager,
    IOAuthSessionManager sessionManager,
    IOAuthProviderManager providerManager,
    IDeviceManager deviceManager,
    ISignInResolver signInResolver,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<HandleOAuthLoginCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;
    private readonly IRoleManager roleManager = roleManager;
    private readonly IOAuthSessionManager sessionManager = sessionManager;
    private readonly IOAuthProviderManager providerManager = providerManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly ISignInResolver signInResolver = signInResolver;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(HandleOAuthLoginCommand request,
        CancellationToken cancellationToken)
    {
        var authenticationResult = request.AuthenticationResult;
        var items = authenticationResult.Properties.Items;
        var providerName = request.AuthenticationResult.Principal.Identity!.AuthenticationType!;
        var fallbackUri = items["fallbackUri"]!;
        var sessionId = items["sessionId"]!;
        var token = items["token"]!;

        if (!string.IsNullOrEmpty(request.RemoteError)) 
            return Results.InternalServerError(request.RemoteError, fallbackUri);
        
        var provider = await providerManager.FindByNameAsync(providerName, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider {providerName}", fallbackUri);

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
                    { "Subject", $"Account registered with {provider.Name}" },
                },
                Payload = new()
                {
                    { "UserName", user.Username },
                    { "ProviderName", provider.Name }
                }
            };

            await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);

            session.SignType = OAuthSignType.SignUp;
            session.UserId = user.Id;

            var sessionResult = await sessionManager.UpdateAsync(session, cancellationToken);
            if (!sessionResult.Succeeded) return Result.Failure(sessionResult.GetError(), fallbackUri);

            var connectResult = await providerManager.ConnectAsync(user, provider, cancellationToken);
            if(!connectResult.Succeeded) return Result.Failure(connectResult.GetError(), fallbackUri);
            
            var link = UrlGenerator.Url(request.ReturnUri!, new { sessionId = session.Id, token });
            return Result.Success(link);
        }

        var credentials = new Dictionary<string, object>()
        {
            { "Email", email },
            { "FallbackUri", fallbackUri },
            { "ReturnUri", request.ReturnUri! },
            { "ProviderName", providerName },
            { "SessionId", sessionId },
            { "Token", token },
        };
        
        var strategy = signInResolver.Resolve(SignInType.LinkedAccount);
        return await strategy.SignInAsync(credentials, cancellationToken);
    }
}