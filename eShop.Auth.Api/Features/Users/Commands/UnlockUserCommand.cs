using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Users.Commands;

internal sealed record UnlockUserCommand(UnlockUserRequest Request) : IRequest<Result>;

internal sealed class UnlockUserCommandHandler(
    IUserManager userManager,
    ILockoutManager lockoutManager) : IRequestHandler<UnlockUserCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;

    public async Task<Result> Handle(UnlockUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        var lockoutStatus = await lockoutManager.GetStatusAsync(user, cancellationToken);

        if (lockoutStatus.LockoutEnabled)
        {
            var result = await lockoutManager.DisableAsync(user, cancellationToken);

            if (!result.Succeeded)
            {
                return result;
            }

            return Result.Success("User account was successfully unlocked.");
        }

        return Result.Success("User account was not locked out.");
    }
}