using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Users.Commands;

internal sealed record RevokePermissionCommand(RevokePermissionRequest Request)
    : IRequest<Result>;

internal sealed class RemoveUserFromPermissionCommandHandler(
    IPermissionManager permissionManager,
    IUserManager userManager)
    : IRequestHandler<RevokePermissionCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(RevokePermissionCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        var permission = await permissionManager.FindByNameAsync(request.Request.PermissionName, cancellationToken);

        if (permission is null)
        {
            return Results.NotFound($"Cannot find permission {request.Request.PermissionName}.");
        }

        var hasUserPermission =
            await permissionManager.HasAsync(user, permission.Name, cancellationToken);

        if (!hasUserPermission)
        {
            return Result.Success("User does not have the permission.");
        }

        var permissionResult =
            await permissionManager.RevokeAsync(user, permission, cancellationToken);

        if (!permissionResult.Succeeded)
        {
            return permissionResult;
        }

        return Result.Success("Successfully removed user form permission.");
    }
}