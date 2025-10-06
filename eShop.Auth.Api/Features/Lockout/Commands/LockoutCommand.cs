using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.Lockout.Commands;

public record LockoutCommand(LockoutRequest Request) : IRequest<Result>;

public class LockoutCommandHandler(
    IUserManager userManager,
    ILockoutManager lockoutManager,
    IReasonManager reasonManager) : IRequestHandler<LockoutCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IReasonManager reasonManager = reasonManager;

    public async Task<Result> Handle(LockoutCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var reason = await reasonManager.FindByIdAsync(request.Request.ReasonId, cancellationToken);
        if (reason is null) return Results.NotFound($"Cannot find reason with ID {request.Request.ReasonId}.");
        
        var description = request.Request.Description;
        var isPermanent = request.Request.IsPermanent;
        var duration = request.Request.Duration;
        var endDate = request.Request.EndDate;
        
        var result = await lockoutManager.BlockAsync(user, reason, description, 
            isPermanent, duration, endDate, cancellationToken);
        
        return result;
    }
}