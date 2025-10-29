using eSystem.Auth.Api.Data.Entities;
using eSystem.Auth.Api.Security.Authorization.Devices;
using eSystem.Auth.Api.Security.Authorization.Permissions;
using eSystem.Auth.Api.Security.Authorization.Roles;
using eSystem.Auth.Api.Security.Identity.Options;
using eSystem.Auth.Api.Security.Identity.User;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;
using eSystem.Core.Security.Identity.Email;

namespace eSystem.Auth.Api.Features.Security.Commands;

public sealed record RegisterCommand(RegistrationRequest Request) : IRequest<Result>;

public sealed class RegisterCommandHandler(
    IPermissionManager permissionManager,
    IUserManager userManager,
    IRoleManager roleManager,
    IDeviceManager deviceManager,
    IHttpContextAccessor httpContextAccessor,
    IOptions<AccountOptions> options) : IRequestHandler<RegisterCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IUserManager userManager = userManager;
    private readonly IRoleManager roleManager = roleManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly AccountOptions options = options.Value;

    public async Task<Result> Handle(RegisterCommand request,
        CancellationToken cancellationToken)
    {
        if (options.RequireUniqueEmail)
        {
            var isTaken = await userManager.IsEmailTakenAsync(request.Request.Email, cancellationToken);
            if (isTaken) return Results.BadRequest("This email address is already taken");
        }

        if (options.RequireUniqueUserName)
        {
            var isUserNameTaken = await userManager.IsUsernameTakenAsync(request.Request.Username, cancellationToken);
            if (isUserNameTaken) return Results.NotFound("Username is already taken");
        }

        var user = new UserEntity()
        {
            Id = Guid.CreateVersion7(),
            Username = request.Request.Username,
            NormalizedUsername = request.Request.Username.ToUpperInvariant(),
        };

        var registrationResult = await userManager.CreateAsync(user, request.Request.Password, cancellationToken);
        if (!registrationResult.Succeeded) return registrationResult;

        var setResult = await userManager.SetEmailAsync(user, request.Request.Email,
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

        var response = new RegistrationResponse()
        {
            UserId = user.Id,
        };

        return Result.Success(response);
    }
}