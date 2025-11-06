using eSecurity.Common.DTOs;
using eSecurity.Security.Identity.User;

namespace eSecurity.Features.Users.Queries;

public record GetUserLockoutQuery(Guid Id) : IRequest<Result>;

public class GetLockoutStateQueryHandler(IUserManager userManager) : IRequestHandler<GetUserLockoutQuery, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserLockoutQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Id}.");
        
        var response = new LockoutStateDto()
        {
            Id = user.LockoutState.Id,
            Type = user.LockoutState.Type,
            Enabled = user.LockoutState.Enabled,
            Permanent = user.LockoutState.Permanent,
            Description = user.LockoutState.Description,
            EndDate = user.LockoutState.EndDate,
            StartDate = user.LockoutState.StartDate,
        };
        
        return Result.Success(response);
    }
}