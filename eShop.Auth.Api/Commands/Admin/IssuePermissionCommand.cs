namespace eShop.Auth.Api.Commands.Admin;

internal sealed record IssuePermissionCommand(IssuePermissionRequest Request)
    : IRequest<Result<IssuePermissionsResponse>>;

internal sealed class IssuePermissionCommandHandler(
    AppManager appManager) : IRequestHandler<IssuePermissionCommand, Result<IssuePermissionsResponse>>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result<IssuePermissionsResponse>> Handle(IssuePermissionCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByIdAsync(request.Request.UserId);

        if (user is null)
        {
            return new(new NotFoundException($"Cannot find user with ID {request.Request.UserId}."));
        }

        var permissions = new List<PermissionEntity>();

        foreach (var permissionName in request.Request.Permissions)
        {
            var permission = await appManager.PermissionManager.FindPermissionAsync(permissionName);

            if (permission is null)
            {
                return new(new NotFoundException($"Cannot find permission {permissionName}."));
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
                    return new(new FailedOperationException(
                        $"Failed on issuing permission with message: {result.Errors.First().Description}"));
                }
            }
        }

        return new(new IssuePermissionsResponse()
            { Succeeded = true, Message = "Successfully issued permissions." });
    }
}