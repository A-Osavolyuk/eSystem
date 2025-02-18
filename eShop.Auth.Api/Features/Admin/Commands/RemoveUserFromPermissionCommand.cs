namespace eShop.Auth.Api.Features.Admin.Commands;

internal sealed record RemoveUserFromPermissionCommand(RemoveUserFromPermissionRequest Request)
    : IRequest<Result<RemoveUserFromPermissionResponse>>;

internal sealed class RemoveUserFromPermissionCommandHandler(
    AppManager appManager)
    : IRequestHandler<RemoveUserFromPermissionCommand, Result<RemoveUserFromPermissionResponse>>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result<RemoveUserFromPermissionResponse>> Handle(RemoveUserFromPermissionCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByIdAsync(request.Request.UserId);

        if (user is null)
        {
            return new(new NotFoundException($"Cannot find user with ID {request.Request.UserId}."));
        }

        var permission = await appManager.PermissionManager.FindPermissionAsync(request.Request.PermissionName);

        if (permission is null)
        {
            return new(new NotFoundException($"Cannot find permission {request.Request.PermissionName}."));
        }

        var hasUserPermission =
            await appManager.PermissionManager.HasPermissionAsync(user, permission.Name);

        if (!hasUserPermission)
        {
            return new(new RemoveUserFromPermissionResponse()
            {
                Succeeded = true,
                Message = "User does not have the permission."
            });
        }
        else
        {
            var permissionResult =
                await appManager.PermissionManager.RemoveFromPermissionAsync(user, permission);

            if (!permissionResult.Succeeded)
            {
                return new(new FailedOperationException(
                    $"Cannot remove user from permission {permission.Name} " +
                    $"due to server error: {permissionResult.Errors.First().Description}."));
            }

            return new(new RemoveUserFromPermissionResponse()
            {
                Succeeded = true,
                Message = "Successfully removed user form permission."
            });
        }
    }
}