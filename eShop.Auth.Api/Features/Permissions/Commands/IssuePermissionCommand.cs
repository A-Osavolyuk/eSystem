using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Permissions.Commands;

internal sealed record IssuePermissionCommand(GrantPermissionRequest Request)
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
            var hasPermission = await permissionManager.HasAsync(user, permission.Name, cancellationToken);

            if (!hasPermission)
            {
                var result = await permissionManager.GrantAsync(user, permission, cancellationToken);

                if (!result.Succeeded)
                {
                    return result;
                }
            }
        }

        return Result.Success("Successfully issued permissions.");
    }
}