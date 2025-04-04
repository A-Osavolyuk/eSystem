namespace eShop.Auth.Api.Features.Admin.Commands;

internal sealed record RemoveUserRolesCommand(RemoveUserRolesRequest Request)
    : IRequest<Result>;

internal sealed class RemoveUserRolesCommandHandler(
    AppManager appManager)
    : IRequestHandler<RemoveUserRolesCommand, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(RemoveUserRolesCommand request,
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

        foreach (var role in request.Request.Roles)
        {
            var isInRole = await appManager.UserManager.IsInRoleAsync(user, role);

            if (!isInRole)
            {
                return Result.Failure(new Error()
                {
                    Code = ErrorCode.BadRequest,
                    Message = "User is not in role",
                    Details = $"User with ID {request.Request.UserId} is not in role {role}."
                });
            }

            var result = await appManager.UserManager.RemoveFromRoleAsync(user, role);

            if (!result.Succeeded)
            {
                return Result.Failure(new Error()
                {
                    Code = ErrorCode.InternalServerError,
                    Message = "Server error",
                    Details = $"Cannot remove role from user with ID {request.Request.UserId} " +
                              $"due to server error: {result.Errors.First().Description}."
                });
            }
        }

        return Result.Success("Roles were successfully removed from user");
    }
}