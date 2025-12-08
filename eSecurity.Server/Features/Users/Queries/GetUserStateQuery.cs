using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.Phone;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserStateQuery(Guid UserId) : IRequest<Result>;

public class GetUserStateQueryHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    IPhoneManager phoneManager) : IRequestHandler<GetUserStateQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly IPhoneManager _phoneManager = phoneManager;

    public async Task<Result> Handle(GetUserStateQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var email = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
        var phoneNumber = await _phoneManager.FindByTypeAsync(user, PhoneNumberType.Primary, cancellationToken);
        var response = new UserStateDto()
        {
            UserId = user.Id,
            Username = user.Username,
            Email = email?.Email,
            PhoneNumber = phoneNumber?.PhoneNumber,
            LockedOut = user.LockoutState.Enabled,
            Roles = user.Roles.Select(x => new RoleDto()
            {
                Id = x.Role.Id,
                Name = x.Role.Name,
                NormalizedName = x.Role.NormalizedName
            }).ToList(),
            Permissions = user.Permissions.Select(x => new PermissionDto
            {
                Id = x.Permission.Id,
                Name = x.Permission.Name
            }).ToList(),
        };
        
        return Results.Ok(response);
    }
}