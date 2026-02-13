using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Results;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserLockoutQuery(Guid Id) : IRequest<Result>;

public class GetLockoutStateQueryHandler(
    IUserManager userManager,
    ILockoutManager lockoutManager) : IRequestHandler<GetUserLockoutQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ILockoutManager _lockoutManager = lockoutManager;

    public async Task<Result> Handle(GetUserLockoutQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var lockoutState = await _lockoutManager.GetAsync(user, cancellationToken);
        if (lockoutState is null) return Results.NotFound("State not found");
        
        var response = new LockoutStateDto
        {
            Id = lockoutState.Id,
            Type = lockoutState.Type,
            Enabled = lockoutState.Enabled,
            Permanent = lockoutState.Permanent,
            Description = lockoutState.Description,
            EndDate = lockoutState.EndDate,
            StartDate = lockoutState.StartDate,
        };
        
        return Results.Ok(response);
    }
}