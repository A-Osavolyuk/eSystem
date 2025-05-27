using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Lockout.Commands;

public record LockoutCommand(LockoutRequest Request) : IRequest<Result>;

public class LockoutCommandHandler(
    IUserManager userManager,
    ILockoutManager lockoutManager) : IRequestHandler<LockoutCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;

    public async Task<Result> Handle(LockoutCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        var result = await lockoutManager.LockoutAsync(user, request.Request.Reason, request.Request.Description,
            request.Request.Permanent, request.Request.LockoutEnd, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }
        
        return Result.Success();
    }
}