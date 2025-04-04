namespace eShop.Auth.Api.Features.Admin.Commands;

internal sealed record RemoveUserRoleCommand(RemoveUserRoleRequest Request) : IRequest<Result>;

internal sealed class RemoveUserRoleCommandHandler(
    AppManager appManager) : IRequestHandler<RemoveUserRoleCommand, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(RemoveUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByIdAsync(request.Request.UserId);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found.",
                Details = $"Cannot find user with ID {request.Request.UserId}."
            });
        }

        var isInRole = await appManager.UserManager.IsInRoleAsync(user, request.Request.Role);

        if (!isInRole)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.BadRequest,
                Message = "User is not in role.",
                Details = $"User with ID {user.Id} not in role {request.Request.Role}"
            });
        }

        var result = await appManager.UserManager.RemoveFromRoleAsync(user, request.Request.Role);

        if (!result.Succeeded)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.InternalServerError,
                Message = "Server error",
                Details = $"Cannot remove user with ID {user.Id} from role {request.Request.Role}."
            });
        }

        return Result.Success("User was successfully removed from role.");
    }
}