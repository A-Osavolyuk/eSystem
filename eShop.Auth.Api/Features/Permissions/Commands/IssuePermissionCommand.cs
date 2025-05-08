using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Permissions.Commands;

internal sealed record IssuePermissionCommand(IssuePermissionRequest Request)
    : IRequest<Result>;

internal sealed class IssuePermissionCommandHandler(
    IPermissionManager permissionManager,
    IUserManager userManager) : IRequestHandler<IssuePermissionCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(IssuePermissionCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound("User does not exist.");
        }

        var permissions = new List<PermissionEntity>();

        foreach (var permissionName in request.Request.Permissions)
        {
            var permission = await permissionManager.FindByNameAsync(permissionName, cancellationToken);

            if (permission is null)
            {
                return Results.NotFound($"Permission {permissionName} does not exist.");
            }

            permissions.Add(permission);
        }

        foreach (var permission in permissions)
        {
            var alreadyHasPermission =
                await permissionManager.HasPermissionAsync(user, permission.Name, cancellationToken);

            if (!alreadyHasPermission)
            {
                var result = await permissionManager.IssueAsync(user, permission.Name, cancellationToken);

                if (!result.Succeeded)
                {
                    return Results.InternalServerError(
                        $"Failed on issuing permission with message: {result.Errors.First().Description}");
                }
            }
        }

        return Result.Success("Successfully issued permissions.");
    }
}