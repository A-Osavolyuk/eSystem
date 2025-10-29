using eSystem.Auth.Api.Security.Identity.User;
using eSystem.Core.DTOs;
using eSystem.Core.Security.Identity.Email;
using eSystem.Core.Security.Identity.PhoneNumber;

namespace eSystem.Auth.Api.Features.Users.Queries;

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
            Roles = user.Roles.Select(x => Mapper.Map(x.Role)).ToList(),
            Permissions = user.Permissions.Select(x => Mapper.Map(x.Permission)).ToList(),
        };
        
        return Result.Success(response);
    }
}