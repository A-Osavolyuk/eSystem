using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record RegisterCommand(RegistrationRequest Request, HttpContext Context) : IRequest<Result>;

public sealed class RegisterCommandHandler(
    IPermissionManager permissionManager,
    IUserManager userManager,
    IRoleManager roleManager,
    IDeviceManager deviceManager,
    IdentityOptions identityOptions) : IRequestHandler<RegisterCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IUserManager userManager = userManager;
    private readonly IRoleManager roleManager = roleManager;
    private readonly IdentityOptions identityOptions = identityOptions;
    private readonly IDeviceManager deviceManager = deviceManager;

    public async Task<Result> Handle(RegisterCommand request,
        CancellationToken cancellationToken)
    {
        if (identityOptions.Account.RequireUniqueEmail)
        {
            var isTaken = await userManager.IsEmailTakenAsync(request.Request.Email, cancellationToken);
            if (isTaken) return Results.BadRequest("This email address is already taken");
        }

        if (identityOptions.Account.RequireUniqueUserName)
        {
            var isUserNameTaken = await userManager.IsUsernameTakenAsync(request.Request.UserName, cancellationToken);
            if (isUserNameTaken) return Results.NotFound("Username is already taken");
        }

        var user = Mapper.Map(request.Request);
        
        var registrationResult = await userManager.CreateAsync(user, request.Request.Password, cancellationToken);
        if (!registrationResult.Succeeded) return registrationResult;

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
        
        var userAgent = RequestUtils.GetUserAgent(request.Context);
        var ipAddress = RequestUtils.GetIpV4(request.Context);
        var clientInfo = RequestUtils.GetClientInfo(request.Context);

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
            UserName = user.Username,
            Email = user.Email,
        };

        return Result.Success(response, "Your account have been successfully registered.");
    }
}