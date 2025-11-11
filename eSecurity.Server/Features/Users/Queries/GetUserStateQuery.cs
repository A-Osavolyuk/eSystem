using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserStateQuery(Guid UserId) : IRequest<Result>;

public class GetUserStateQueryHandler(IUserManager userManager) : IRequestHandler<GetUserStateQuery, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserStateQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var response = new UserStateDto()
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.GetEmail(EmailType.Primary)?.Email,
            PhoneNumber = user.GetPhoneNumber(PhoneNumberType.Primary)?.PhoneNumber,
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
        
        return Result.Success(response);
    }
}