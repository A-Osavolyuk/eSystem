using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Authorization.Permissions;
using eSecurity.Server.Security.Authorization.Roles;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Http.Context;

namespace eSecurity.Server.Security.Identity.SignUp.Strategies;

public sealed class ManualSignUpPayload : SignUpPayload
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
}

public sealed class ManualSignUpStrategy(
    IUserManager userManager,
    IPasswordManager passwordManager,
    IPermissionManager permissionManager,
    IRoleManager roleManager,
    IDeviceManager deviceManager,
    IEmailManager emailManager,
    IHttpContextAccessor httpContextAccessor,
    IOptions<AccountOptions> options) : ISignUpStrategy
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly IPermissionManager _permissionManager = permissionManager;
    private readonly IRoleManager _roleManager = roleManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly AccountOptions _options = options.Value;

    public async ValueTask<Result> ExecuteAsync(SignUpPayload payload, 
        CancellationToken cancellationToken = default)
    {
        if (payload is not ManualSignUpPayload manualPayload)
            return Results.BadRequest("Incorrect payload type");
        
        if (_options.RequireUniqueEmail)
        {
            var isTaken = await _emailManager.IsTakenAsync(manualPayload.Email, cancellationToken);
            if (isTaken) return Results.BadRequest("This email address is already taken");
        }

        if (_options.RequireUniqueUserName)
        {
            var isUserNameTaken = await _userManager.IsUsernameTakenAsync(manualPayload.Username, cancellationToken);
            if (isUserNameTaken) return Results.NotFound("Username is already taken");
        }

        var user = new UserEntity()
        {
            Id = Guid.CreateVersion7(),
            Username = manualPayload.Username,
            NormalizedUsername = manualPayload.Username.ToUpperInvariant(),
        };

        var createResult = await _userManager.CreateAsync(user, cancellationToken);
        if (!createResult.Succeeded) return createResult;
        
        var passwordResult = await _passwordManager.AddAsync(user, manualPayload.Password, cancellationToken);
        if (!passwordResult.Succeeded) return passwordResult;

        var setResult = await _emailManager.SetAsync(user, manualPayload.Email,
            EmailType.Primary, cancellationToken);

        if (setResult.Succeeded) return setResult;

        var role = await _roleManager.FindByNameAsync("User", cancellationToken);
        if (role is null) return Results.NotFound("Cannot find role with name User");

        var assignRoleResult = await _roleManager.AssignAsync(user, role, cancellationToken);
        if (!assignRoleResult.Succeeded) return assignRoleResult;

        if (role.Permissions.Count > 0)
        {
            var permissions = role.Permissions.Select(x => x.Permission).ToList();

            foreach (var permission in permissions)
            {
                var grantResult = await _permissionManager.GrantAsync(user, permission, cancellationToken);
                if (!grantResult.Succeeded) return grantResult;
            }
        }

        var userAgent = _httpContextAccessor.HttpContext?.GetUserAgent()!;
        var ipAddress = _httpContextAccessor.HttpContext?.GetIpV4()!;
        var clientInfo = _httpContextAccessor.HttpContext?.GetClientInfo()!;

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
            FirstSeen = DateTimeOffset.UtcNow,
            CreateDate = DateTimeOffset.UtcNow
        };

        var deviceResult = await _deviceManager.CreateAsync(newDevice, cancellationToken);
        if (!deviceResult.Succeeded) return deviceResult;

        var response = new SignUpResponse()
        {
            UserId = user.Id,
        };

        return Results.Ok(response);
    }
}