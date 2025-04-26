using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Users.Commands;

internal sealed record UnlockUserCommand(UnlockUserRequest Request) : IRequest<Result>;

internal sealed class UnlockUserCommandHandler(
    UserManager<UserEntity> userManager) : IRequestHandler<UnlockUserCommand, Result>
{
    private readonly UserManager<UserEntity> userManager = userManager;

    public async Task<Result> Handle(UnlockUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        var lockoutStatus = await userManager.GetLockoutStatusAsync(user);

        if (lockoutStatus.LockoutEnabled)
        {
            var result = await userManager.UnlockUserAsync(user);

            if (!result.Succeeded)
            {
                return Results.InternalServerError($"Cannot unlock user with ID {request.Request.UserId} " +
                                                   $"due to server error: {result.Errors.First().Description}.");
            }

            return Result.Success("User account was successfully unlocked.");
        }

        return Result.Success("User account was not locked out.");
    }
}