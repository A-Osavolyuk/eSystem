using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Authorization.Permissions;
using eSecurity.Server.Security.Authorization.Roles;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.Phone;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserStateQuery(Guid UserId) : IRequest<Result>;

public class GetUserStateQueryHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    ILockoutManager lockoutManager,
    IPhoneManager phoneManager,
    IPermissionManager permissionManager,
    IRoleManager roleManager) : IRequestHandler<GetUserStateQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly ILockoutManager _lockoutManager = lockoutManager;
    private readonly IPhoneManager _phoneManager = phoneManager;
    private readonly IPermissionManager _permissionManager = permissionManager;
    private readonly IRoleManager _roleManager = roleManager;

    public async Task<Result> Handle(GetUserStateQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var lockoutState = await _lockoutManager.GetAsync(user, cancellationToken);
        if (lockoutState is null) return Results.NotFound("State not found");
        
        var email = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
        var phoneNumber = await _phoneManager.FindByTypeAsync(user, PhoneNumberType.Primary, cancellationToken);
        var roles = await _roleManager.GetAllAsync(user, cancellationToken);
        var permissions = await _permissionManager.GetAllAsync(user, cancellationToken);
        
        var response = new UserStateDto()
        {
            UserId = user.Id,
            Username = user.Username,
            Email = email?.Email,
            LockedOut = lockoutState.Enabled,
            PhoneNumber = phoneNumber?.PhoneNumber,
            Roles = roles.Select(x => new RoleDto()
            {
                Id = x.Role.Id,
                Name = x.Role.Name,
                NormalizedName = x.Role.NormalizedName
            }).ToList(),
            Permissions = permissions.Select(x => new PermissionDto
            {
                Id = x.Permission.Id,
                Name = x.Permission.Name
            }).ToList(),
        };
        
        return Results.Ok(response);
    }
}