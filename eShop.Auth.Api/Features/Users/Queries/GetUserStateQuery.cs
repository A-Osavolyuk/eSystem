using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Features.Users.Queries;

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
            Email = user.Email,
            RecoveryEmail = user.RecoveryEmail,
            PhoneNumber = user.PhoneNumber,
            Username = user.UserName,
            LockedOut = user.LockoutState.Enabled
        };
        
        return Result.Success(response);
    }
}