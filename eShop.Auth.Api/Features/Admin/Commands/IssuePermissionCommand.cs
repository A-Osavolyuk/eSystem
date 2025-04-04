namespace eShop.Auth.Api.Features.Admin.Commands;

internal sealed record IssuePermissionCommand(IssuePermissionRequest Request)
    : IRequest<Result>;

internal sealed class IssuePermissionCommandHandler(
    AppManager appManager) : IRequestHandler<IssuePermissionCommand, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(IssuePermissionCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByIdAsync(request.Request.UserId);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"User does not exist."
            });
        }

        var permissions = new List<PermissionEntity>();

        foreach (var permissionName in request.Request.Permissions)
        {
            var permission = await appManager.PermissionManager.FindPermissionAsync(permissionName);

            if (permission is null)
            {
                return Result.Failure(new Error()
                {
                    Code = ErrorCode.NotFound,
                    Message = "Not found",
                    Details = $"Permission {permissionName} does not exist."
                });
            }

            permissions.Add(permission);
        }

        foreach (var permission in permissions)
        {
            var alreadyHasPermission = await appManager.PermissionManager.HasPermissionAsync(user, permission.Name);

            if (!alreadyHasPermission)
            {
                var result = await appManager.PermissionManager.IssuePermissionAsync(user, permission.Name);

                if (!result.Succeeded)
                {
                    return Result.Failure(new Error()
                    {
                        Code = ErrorCode.InternalServerError,
                        Message = "Server error",
                        Details = $"Failed on issuing permission with message: {result.Errors.First().Description}"
                    });
                }
            }
        }

        return Result.Success("Successfully issued permissions.");
    }
}