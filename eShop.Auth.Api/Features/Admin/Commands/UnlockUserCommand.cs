using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Admin.Commands;

internal sealed record UnlockUserCommand(UnlockUserRequest Request) : IRequest<Result>;

internal sealed class UnlockUserCommandHandler(
    AppManager appManager) : IRequestHandler<UnlockUserCommand, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(UnlockUserCommand request,
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

        var lockoutStatus = await appManager.UserManager.GetLockoutStatusAsync(user);

        if (lockoutStatus.LockoutEnabled)
        {
            var result = await appManager.UserManager.UnlockUserAsync(user);

            if (!result.Succeeded)
            {
                return Result.Failure(new Error()
                {
                    Code = ErrorCode.InternalServerError,
                    Message = "Server error",
                    Details = $"Cannot unlock user with ID {request.Request.UserId} " +
                              $"due to server error: {result.Errors.First().Description}."
                });
            }

            return Result.Success("User account was successfully unlocked.");
        }
        else
        {
            return Result.Success("User account was not locked out.");
        }
    }
}