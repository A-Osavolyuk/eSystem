using eSecurity.Common.Responses;
using eSecurity.Data.Entities;
using eSecurity.Security.Authentication.Password;
using eSecurity.Security.Authorization.Devices;
using eSecurity.Security.Authorization.Permissions;
using eSecurity.Security.Authorization.Roles;
using eSecurity.Security.Identity.Options;
using eSecurity.Security.Identity.User;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Security.Identity.Email;

namespace eSecurity.Security.Identity.SignUp.Strategies;

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
    IHttpContextAccessor httpContextAccessor,
    IOptions<AccountOptions> options) : ISignUpStrategy
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasswordManager passwordManager = passwordManager;
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IRoleManager roleManager = roleManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly AccountOptions options = options.Value;

    public async ValueTask<Result> SignUpAsync(SignUpPayload payload, 
        CancellationToken cancellationToken = default)
    {
        if (payload is not ManualSignUpPayload manualPayload)
            return Results.BadRequest("Incorrect payload type");
        
        if (options.RequireUniqueEmail)
        {
            var isTaken = await userManager.IsEmailTakenAsync(manualPayload.Email, cancellationToken);
            if (isTaken) return Results.BadRequest("This email address is already taken");
        }

        if (options.RequireUniqueUserName)
        {
            var isUserNameTaken = await userManager.IsUsernameTakenAsync(manualPayload.Username, cancellationToken);
            if (isUserNameTaken) return Results.NotFound("Username is already taken");
        }

        var user = new UserEntity()
        {
            Id = Guid.CreateVersion7(),
            Username = manualPayload.Username,
            NormalizedUsername = manualPayload.Username.ToUpperInvariant(),
        };

        var createResult = await userManager.CreateAsync(user, cancellationToken);
        if (!createResult.Succeeded) return createResult;
        
        var passwordResult = await passwordManager.AddAsync(user, manualPayload.Password, cancellationToken);
        if (!passwordResult.Succeeded) return passwordResult;

        var setResult = await userManager.SetEmailAsync(user, manualPayload.Email,
            EmailType.Primary, cancellationToken);

        if (setResult.Succeeded) return setResult;

        var role = await roleManager.FindByNameAsync("User", cancellationToken);
        if (role is null) return Results.NotFound("Cannot find role with name User");

        var assignRoleResult = await roleManager.AssignAsync(user, role, cancellationToken);
        if (!assignRoleResult.Succeeded) return assignRoleResult;

        if (role.Permissions.Count > 0)
        {
            var permissions = role.Permissions.Select(x => x.Permission).ToList();

            foreach (var permission in permissions)
            {
                var grantResult = await permissionManager.GrantAsync(user, permission, cancellationToken);
                if (!grantResult.Succeeded) return grantResult;
            }
        }

        var userAgent = httpContextAccessor.HttpContext?.GetUserAgent()!;
        var ipAddress = httpContextAccessor.HttpContext?.GetIpV4()!;
        var clientInfo = httpContextAccessor.HttpContext?.GetClientInfo()!;

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
        if (!deviceResult.Succeeded) return deviceResult;

        var response = new SignUpResponse()
        {
            UserId = user.Id,
        };

        return Result.Success(response);
    }
}