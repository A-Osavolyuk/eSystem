using System.Security.Claims;
using eShop.Auth.Api.Messages.Email;
using eShop.Auth.Api.Types;

namespace eShop.Auth.Api.Features.LinkedAccounts.Commands;

public sealed record HandleOAuthLoginCommand(
    HttpContext Context,
    AuthenticationResult AuthenticationResult,
    string? RemoteError,
    string? ReturnUri) : IRequest<Result>;

public sealed class HandleOAuthLoginCommandHandler(
    IPermissionManager permissionManager,
    IUserManager userManager,
    IMessageService messageService,
    IRoleManager roleManager,
    ILockoutManager lockoutManager,
    IOAuthSessionManager sessionManager,
    IOAuthProviderManager providerManager,
    ILoginSessionManager loginSessionManager,
    IDeviceManager deviceManager) : IRequestHandler<HandleOAuthLoginCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;
    private readonly IRoleManager roleManager = roleManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IOAuthSessionManager sessionManager = sessionManager;
    private readonly IOAuthProviderManager providerManager = providerManager;
    private readonly ILoginSessionManager loginSessionManager = loginSessionManager;
    private readonly IDeviceManager deviceManager = deviceManager;

    public async Task<Result> Handle(HandleOAuthLoginCommand request,
        CancellationToken cancellationToken)
    {
        var authenticationResult = request.AuthenticationResult;
        var items = authenticationResult.Properties.Items;
        var providerName = request.AuthenticationResult.Principal.Identity!.AuthenticationType!;
        var fallbackUri = items["fallbackUri"]!;

        if (!string.IsNullOrEmpty(request.RemoteError))
        {
            return RedirectWithError(OAuthErrorType.RemoteError,
                providerName, request.RemoteError, fallbackUri);
        }

        var provider = await providerManager.FindByNameAsync(providerName, cancellationToken);

        if (provider is null)
        {
            return RedirectWithError(OAuthErrorType.InternalError,
                providerName, $"Cannot find provider {providerName}", fallbackUri);
        }

        var email = request.AuthenticationResult.Principal.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        if (email is null)
        {
            return RedirectWithError(OAuthErrorType.InvalidCredentials,
                provider.Name, "Email was not provided to credentials", fallbackUri);
        }

        var sessionId = Guid.Parse(items["sessionId"]!);
        var token = items["token"]!;

        var session = await sessionManager.FindAsync(sessionId, token, cancellationToken);

        if (session is null)
        {
            return RedirectWithError(OAuthErrorType.InvalidCredentials,
                provider.Name, "Invalid OAuth session", fallbackUri);
        }

        var user = await userManager.FindByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            user = new UserEntity()
            {
                Id = Guid.CreateVersion7(),
                Username = email,
            };

            var createResult = await CreateAccountAsync(user, provider, fallbackUri, cancellationToken);
            if (!createResult.Succeeded) return createResult;

            var setResult = await userManager.SetEmailAsync(user, email, 
                isPrimary: true, cancellationToken: cancellationToken);
            
            if(setResult.Succeeded) return setResult;

            var role = await roleManager.FindByNameAsync("User", cancellationToken);

            if (role is null)
            {
                return RedirectWithError(OAuthErrorType.InternalError,
                    provider.Name, "Cannot find role with name User", fallbackUri);
            }

            var assignResult = await AssignRoleAsync(user, role, provider, fallbackUri, cancellationToken);
            if (!assignResult.Succeeded) return assignResult;

            if (role.Permissions.Count > 0)
            {
                var grantResult = await GrantPermissionsAsync(user, role, provider, fallbackUri, cancellationToken);
                if (!grantResult.Succeeded) return grantResult;
            }

            var deviceResult = await CreateDeviceAsync(user, provider, request.Context, fallbackUri, cancellationToken);
            if (!deviceResult.Succeeded) return deviceResult;
            
            await SendMessageAsync(user, provider, cancellationToken);

            session.SignType = OAuthSignType.SignUp;
            session.UserId = user.Id;

            var sessionResult = await UpdateSessionAsync(session, provider, fallbackUri, cancellationToken);
            if (!sessionResult.Succeeded) return sessionResult;

            return await SignUpAsync(user, provider, session,
                request.ReturnUri!, fallbackUri, token, cancellationToken);
        }

        var userAgent = RequestUtils.GetUserAgent(request.Context);
        var ipAddress = RequestUtils.GetIpV4(request.Context);
        var clientInfo = RequestUtils.GetClientInfo(request.Context);

        var device = await deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);

        if (device is null)
        {
            device = new UserDeviceEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                UserAgent = userAgent,
                IpAddress = ipAddress,
                Browser = clientInfo.UA.ToString(),
                OS = clientInfo.OS.ToString(),
                Device = clientInfo.Device.ToString(),
                IsTrusted = false,
                IsBlocked = false,
                FirstSeen = DateTimeOffset.UtcNow,
                CreateDate = DateTimeOffset.UtcNow
            };

            var result = await deviceManager.CreateAsync(device, cancellationToken);
            if (!result.Succeeded) return result;
        }

        if (device.IsBlocked)
        {
            return RedirectWithError(OAuthErrorType.InternalError,
                provider.Name, "Cannot sign in with this device, device is blocked", fallbackUri);
        }

        if (!device.IsTrusted)
        {
            var deviceResult = await TrustDeviceAsync(device, provider, fallbackUri, cancellationToken);
            if (!deviceResult.Succeeded) return deviceResult;
        }

        var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);

        if (lockoutState.Enabled) return LockedOut(lockoutState, provider, fallbackUri);

        session.SignType = OAuthSignType.SignIn;
        session.UserId = user.Id;

        var updateResult = await UpdateSessionAsync(session, provider, fallbackUri, cancellationToken);
        if (!updateResult.Succeeded) return updateResult;

        var providerAllowedResult = IsProviderAllowed(user, provider, fallbackUri);
        if (!providerAllowedResult.Succeeded) return providerAllowedResult;

        var checkProviderResult = await CheckUserProviderAsync(user, provider, fallbackUri, cancellationToken);
        if (!checkProviderResult.Succeeded) return checkProviderResult;

        return await SignInAsync( provider, session, device, request.ReturnUri!, token, cancellationToken);
    }

    private async Task<Result> CreateDeviceAsync(UserEntity user, OAuthProviderEntity provider,
        HttpContext context, string fallbackUri, CancellationToken cancellationToken)
    {
        var userAgent = RequestUtils.GetUserAgent(context);
        var ipAddress = RequestUtils.GetIpV4(context);
        var clientInfo = RequestUtils.GetClientInfo(context);

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

        var result = await deviceManager.CreateAsync(newDevice, cancellationToken);
        if (!result.Succeeded)
            return RedirectWithError(OAuthErrorType.InternalError,
                provider.Name, result.Message, fallbackUri);

        return Result.Success();
    }

    private async Task<Result> TrustDeviceAsync(UserDeviceEntity device, OAuthProviderEntity provider,
        string fallbackUri, CancellationToken token)
    {
        var result = await deviceManager.TrustAsync(device, token);
        
        if (!result.Succeeded)
            return RedirectWithError(OAuthErrorType.InternalError,
                provider.Name, result.Message, fallbackUri);

        return Result.Success();
    }

    private Result RedirectWithError(OAuthErrorType type, string provider, string message, string fallbackUri)
    {
        var error = Uri.EscapeDataString(message);
        var url = UrlGenerator.Url(fallbackUri, new
        {
            ErrorCode = type.ToString(),
            Message = error,
            Provider = provider
        });

        return Results.Redirect(url);
    }

    private async Task<Result> CreateAccountAsync(UserEntity user, OAuthProviderEntity provider,
        string fallbackUri, CancellationToken cancellationToken)
    {
        var result = await userManager.CreateAsync(user, cancellationToken: cancellationToken);
        
        if (!result.Succeeded)
            return RedirectWithError(OAuthErrorType.InternalError,
                provider.Name, result.Message, fallbackUri);

        return Result.Success();
    }

    private async Task<Result> AssignRoleAsync(UserEntity user, RoleEntity role, OAuthProviderEntity provider,
        string fallbackUri, CancellationToken cancellationToken)
    {
        var result = await roleManager.AssignAsync(user, role, cancellationToken);

        if (!result.Succeeded)
            return RedirectWithError(OAuthErrorType.InternalError,
                provider.Name, result.Message, fallbackUri);

        return Result.Success();
    }

    private async Task<Result> GrantPermissionsAsync(UserEntity user, RoleEntity role, OAuthProviderEntity provider,
        string fallbackUri, CancellationToken cancellationToken)
    {
        var permissions = role.Permissions.Select(x => x.Permission).ToList();

        foreach (var permission in permissions)
        {
            var result = await permissionManager.GrantAsync(user, permission, cancellationToken);

            if (result.Succeeded) continue;
            return RedirectWithError(OAuthErrorType.InternalError, provider.Name, result.Message, fallbackUri);
        }

        return Result.Success();
    }

    private async Task SendMessageAsync(UserEntity user, OAuthProviderEntity provider,
        CancellationToken cancellationToken)
    {
        var message = new OAuthSignUpMessage()
        {
            Credentials = new Dictionary<string, string>()
            {
                { "To", user.Emails.First(x => x.IsPrimary).Email },
                { "Subject", $"Account registered with {provider.Name}" },
            },
            Payload = new()
            {
                { "UserName", user.Username },
                { "ProviderName", provider.Name }
            }
        };

        await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);
    }

    private async Task<Result> SignUpAsync(UserEntity user, OAuthProviderEntity provider, OAuthSessionEntity session,
        string returnUri, string fallbackUri, string token, CancellationToken cancellationToken)
    {
        var result = await providerManager.ConnectAsync(user, provider, cancellationToken);

        if (!result.Succeeded)
            return RedirectWithError(OAuthErrorType.InternalError,
                provider.Name, result.Message, fallbackUri);

        var link = UrlGenerator.Url(returnUri, new { sessionId = session.Id, token });

        return Result.Success(link);
    }

    private Result LockedOut(LockoutStateEntity lockoutState, OAuthProviderEntity provider, string fallbackUri)
    {
        var error = Uri.EscapeDataString($"This user account is locked out with reason: {lockoutState.Reason}.");
        return RedirectWithError(OAuthErrorType.InternalError, provider.Name, error, fallbackUri);
    }

    private async Task<Result> UpdateSessionAsync(OAuthSessionEntity session,
        OAuthProviderEntity provider, string fallbackUri, CancellationToken cancellationToken)
    {
        var sessionResult = await sessionManager.UpdateAsync(session, cancellationToken);

        if (sessionResult.Succeeded) return Result.Success();

        return RedirectWithError(OAuthErrorType.InternalError, 
            provider.Name, sessionResult.Message, fallbackUri);

    }

    private Result IsProviderAllowed(UserEntity user, OAuthProviderEntity provider, string fallbackUri)
    {
        if (!user.LinkedAccounts.Any(x => x.ProviderId == provider.Id && !x.Allowed)) 
            return Result.Success();

        var error = Uri.EscapeDataString("OAuth provider is disallowed by user");
        
        return RedirectWithError(OAuthErrorType.Unavailable, provider.Name, error, fallbackUri);

    }

    private async Task<Result> CheckUserProviderAsync(UserEntity user, OAuthProviderEntity provider,
        string fallbackUri, CancellationToken cancellationToken)
    {
        var enableResult = await providerManager.ConnectAsync(user, provider, cancellationToken);

        if (enableResult.Succeeded) return Result.Success();
            
        var error = Uri.EscapeDataString(enableResult.GetError().Message);
        
        return RedirectWithError(OAuthErrorType.InternalError, 
            provider.Name, error, fallbackUri);

    }

    private async Task<Result> SignInAsync(OAuthProviderEntity provider,
        OAuthSessionEntity session, UserDeviceEntity device, string returnUri,
        string token, CancellationToken cancellationToken)
    {
        await loginSessionManager.CreateAsync(device, LoginType.OAuth, provider.Name, cancellationToken);

        var link = UrlGenerator.Url(returnUri, new { sessionId = session.Id, token });

        return Result.Success(link);
    }
}