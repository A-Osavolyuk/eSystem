using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Users.Commands;

internal sealed record UnlockUserCommand(UnlockUserRequest Request) : IRequest<Result>;

internal sealed class UnlockUserCommandHandler(
    IUserManager userManager) : IRequestHandler<UnlockUserCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(UnlockUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        var lockoutStatus = await userManager.GetLockoutStatusAsync(user, cancellationToken);

        if (lockoutStatus.LockoutEnabled)
        {
            var result = await userManager.UnlockUserAsync(user, cancellationToken);

            if (!result.Succeeded)
            {
                return result;
            }

            return Result.Success("User account was successfully unlocked.");
        }

        return Result.Success("User account was not locked out.");
    }
}