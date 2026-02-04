using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Authorization.Roles;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSecurity.Server.Security.Identity.User.Username;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Identity.SignUp.Strategies;

public sealed class ManualSignUpPayload : SignUpPayload
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
}

public sealed class ManualSignUpStrategy(
    IUserManager userManager,
    IUsernameManager usernameManager,
    IPasswordManager passwordManager,
    IRoleManager roleManager,
    IDeviceManager deviceManager,
    IEmailManager emailManager,
    IHttpContextAccessor httpContextAccessor,
    IOptions<AccountOptions> options) : ISignUpStrategy
{
    private readonly IUserManager _userManager = userManager;
    private readonly IUsernameManager _usernameManager = usernameManager;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly IRoleManager _roleManager = roleManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly AccountOptions _options = options.Value;

    public async ValueTask<Result> ExecuteAsync(SignUpPayload payload,
        CancellationToken cancellationToken = default)
    {
        if (payload is not ManualSignUpPayload manualPayload)
            return Results.BadRequest("Incorrect payload type");
        
        if (_options.RequireUniqueUsername && 
            await _usernameManager.IsTakenAsync(manualPayload.Username, cancellationToken))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.UsernameTaken,
                Description = "This username is already taken"
            });
        }

        if (_options.RequireUniqueEmail && 
            await _emailManager.IsTakenAsync(manualPayload.Email, cancellationToken))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.EmailTaken,
                Description = "This email is already taken."
            });
        }

        var user = new UserEntity
        {
            Id = Guid.CreateVersion7(),
            Username = manualPayload.Username,
            NormalizedUsername = manualPayload.Username.ToUpperInvariant(),
            Locale = _httpContext.GetLocale()!,
            ZoneInfo = _httpContext.GetTimeZone()!,
        };

        var createResult = await _userManager.CreateAsync(user, cancellationToken);
        if (!createResult.Succeeded) return createResult;

        var passwordResult = await _passwordManager.AddAsync(user, manualPayload.Password, cancellationToken);
        if (!passwordResult.Succeeded) return passwordResult;

        var setResult = await _emailManager.SetAsync(user, manualPayload.Email,
            EmailType.Primary, cancellationToken);

        if (!setResult.Succeeded) return setResult;

        var role = await _roleManager.FindByNameAsync("User", cancellationToken);
        if (role is null) return Results.NotFound("Cannot find role with name User");

        var assignRoleResult = await _roleManager.AssignAsync(user, role, cancellationToken);
        if (!assignRoleResult.Succeeded) return assignRoleResult;

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

        var response = new SignUpResponse
        {
            UserId = user.Id,
        };

        return Results.Ok(response);
    }
}