using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Lockout.Commands;

public record UnlockCommand(UnlockRequest Request) : IRequest<Result>;

public class UnlockCommandHandler(
    IUserManager userManager,
    ILockoutManager lockoutManager) : IRequestHandler<UnlockCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    
    public async Task<Result> Handle(UnlockCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }
        
        var result = await lockoutManager.UnlockAsync(user, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }
        
        return Result.Success();
    }
}