using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Admin;

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
            return Results.NotFound("User does not exist.");
        }

        var permissions = new List<PermissionEntity>();

        foreach (var permissionName in request.Request.Permissions)
        {
            var permission = await appManager.PermissionManager.FindByNameAsync(permissionName, cancellationToken);

            if (permission is null)
            {
                return Results.NotFound($"Permission {permissionName} does not exist.");
            }

            permissions.Add(permission);
        }

        foreach (var permission in permissions)
        {
            var alreadyHasPermission =
                await appManager.PermissionManager.HasPermissionAsync(user, permission.Name, cancellationToken);

            if (!alreadyHasPermission)
            {
                var result = await appManager.PermissionManager.IssueAsync(user, permission.Name, cancellationToken);

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