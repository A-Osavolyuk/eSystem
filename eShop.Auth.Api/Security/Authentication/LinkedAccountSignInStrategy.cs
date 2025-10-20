namespace eShop.Auth.Api.Security.Authentication;

public class LinkedAccountSignInStrategy(
    IUserManager userManager,
    IDeviceManager deviceManager,
    ILockoutManager lockoutManager,
    IHttpContextAccessor httpContextAccessor,
    IOAuthProviderManager providerManager,
    IOAuthSessionManager sessionManager,
    ILoginManager loginManager,
    IAuthorizationManager authorizationManager) : SignInStrategy
{
    private readonly IUserManager userManager = userManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IOAuthProviderManager providerManager = providerManager;
    private readonly IOAuthSessionManager sessionManager = sessionManager;
    private readonly ILoginManager loginManager = loginManager;
    private readonly IAuthorizationManager authorizationManager = authorizationManager;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public override async ValueTask<Result> SignInAsync(Dictionary<string, object> credentials,
        CancellationToken cancellationToken = default)
    {
        var email = credentials["Email"] as string;
        var fallbackUri = (credentials["FallbackUri"] as string)!;
        var returnUri = credentials["ReturnUri"] as string;
        var providerName = credentials["ProviderName"] as string;
        var sessionId = credentials["SessionId"] as string;
        var token = credentials["Token"] as string;

        if (string.IsNullOrWhiteSpace(email)) return Results.BadRequest("Email is empty", fallbackUri);
        if (string.IsNullOrWhiteSpace(returnUri)) return Results.BadRequest("Return URI is empty", fallbackUri);
        if (string.IsNullOrWhiteSpace(providerName)) return Results.BadRequest("Provider name is empty", fallbackUri);
        if (string.IsNullOrWhiteSpace(token)) return Results.BadRequest("Token name is empty", fallbackUri);
        if (string.IsNullOrWhiteSpace(sessionId)) return Results.BadRequest("Session ID is empty", fallbackUri);

        var user = await userManager.FindByEmailAsync(email, cancellationToken);
        if (user is null) return Results.BadRequest($"Cannot find user with email {email}.");

        var userAgent = httpContext.GetUserAgent();
        var ipAddress = httpContext.GetIpV4();
        var clientInfo = httpContext.GetClientInfo();
        var device = user.GetDevice(userAgent, ipAddress);

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

        var provider = await providerManager.FindByNameAsync(providerName, cancellationToken);
        if (provider is null) return Results.NotFound($"Cannot find provider with name {providerName}.", fallbackUri);
        
        if (device.IsBlocked) return Results.BadRequest("Device is blocked", fallbackUri);
        if (!device.IsTrusted)
        {
            var deviceResult = await deviceManager.TrustAsync(device, cancellationToken);
            if (!deviceResult.Succeeded) return Result.Failure(deviceResult.GetError(), fallbackUri);
        }

        if (user.LockoutState.Enabled)
        {
            var lockoutResult = await lockoutManager.UnblockAsync(user, cancellationToken);
            if (!lockoutResult.Succeeded) return Result.Failure(lockoutResult.GetError(), fallbackUri);
        }

        var session = await sessionManager.FindAsync(Guid.Parse(sessionId), token, cancellationToken);
        if (session is null) return Results.BadRequest("Cannot find session with id {sessionId}.");

        session.SignType = OAuthSignType.SignIn;
        session.UserId = user.Id;

        var updateResult = await sessionManager.UpdateAsync(session, cancellationToken);
        if (!updateResult.Succeeded) return Result.Failure(updateResult.GetError(), fallbackUri);

        var connectResult = await providerManager.ConnectAsync(user, provider, cancellationToken);
        if (!connectResult.Succeeded) return Result.Failure(connectResult.GetError(), fallbackUri);

        await loginManager.CreateAsync(device, LoginType.OAuth, provider.Name, cancellationToken);
        await authorizationManager.CreateAsync(device, cancellationToken);

        var link = UrlGenerator.Url(returnUri, new { sessionId = session.Id, token });
        return Result.Success(link);
    }
}