namespace eShop.Auth.Api.Features.Admin.Commands;

internal sealed record LockoutUserCommand(LockoutUserRequest Request) : IRequest<Result>;

internal sealed class LockoutUserCommandHandler(
    AppManager appManager) : IRequestHandler<LockoutUserCommand, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(LockoutUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByIdAsync(request.Request.UserId);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with ID {request.Request.UserId}."
            });
        }

        if (request.Request.Permanent)
        {
            var lockoutEndDate = DateTime.UtcNow.AddYears(100);
            await appManager.UserManager.SetLockoutEnabledAsync(user, true);
            await appManager.UserManager.SetLockoutEndDateAsync(user, lockoutEndDate);

            return Result.Success(new LockoutUserResponse()
            {
                LockoutEnabled = true,
                LockoutEnd = lockoutEndDate,
                Message = "User was successfully permanently banned.",
                Succeeded = true
            });
        }
        else
        {
            await appManager.UserManager.SetLockoutEnabledAsync(user, true);
            await appManager.UserManager.SetLockoutEndDateAsync(user, request.Request.LockoutEnd);

            return Result.Success(new LockoutUserResponse()
            {
                LockoutEnabled = true,
                LockoutEnd = request.Request.LockoutEnd,
                Message = $"User was successfully banned until {request.Request.LockoutEnd}.",
                Succeeded = true
            });
        }
    }
}