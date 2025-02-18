namespace eShop.Auth.Api.Features.Admin.Commands;

internal sealed record UnlockUserCommand(UnlockUserRequest Request) : IRequest<Result<UnlockUserResponse>>;

internal sealed class UnlockUserCommandHandler(
    AppManager appManager) : IRequestHandler<UnlockUserCommand, Result<UnlockUserResponse>>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result<UnlockUserResponse>> Handle(UnlockUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByIdAsync(request.Request.UserId);

        if (user is null)
        {
            return new(new NotFoundException($"Cannot find user with ID {request.Request.UserId}."));
        }

        var lockoutStatus = await appManager.UserManager.GetLockoutStatusAsync(user);

        if (lockoutStatus.LockoutEnabled)
        {
            var result = await appManager.UserManager.UnlockUserAsync(user);

            if (!result.Succeeded)
            {
                return new(new FailedOperationException(
                    $"Cannot unlock user with ID {request.Request.UserId} " +
                    $"due to server error: {result.Errors.First().Description}."));
            }

            return new(new UnlockUserResponse()
            {
                Succeeded = true,
                Message = "User account was successfully unlocked."
            });
        }
        else
        {
            return new(new UnlockUserResponse()
            {
                Succeeded = true,
                Message = "User account was not locked out."
            });
        }
    }
}