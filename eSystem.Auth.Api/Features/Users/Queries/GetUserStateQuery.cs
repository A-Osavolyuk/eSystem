using eSystem.Auth.Api.Interfaces;
using eSystem.Auth.Api.Mapping;
using eSystem.Domain.DTOs;

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
            PrimaryEmail = user.GetEmail(EmailType.Primary)?.Email,
            RecoveryEmail = user.GetEmail(EmailType.Recovery)?.Email,
            PrimaryPhoneNumber = user.GetPhoneNumber(PhoneNumberType.Primary)?.PhoneNumber,
            RecoveryPhoneNumber = user.GetPhoneNumber(PhoneNumberType.Recovery)?.PhoneNumber,
            LockedOut = user.LockoutState.Enabled,
            Roles = user.Roles.Select(x => Mapper.Map(x.Role)).ToList(),
            Permissions = user.Permissions.Select(x => Mapper.Map(x.Permission)).ToList(),
        };
        
        return Result.Success(response);
    }
}