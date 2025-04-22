using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Admin.Commands;

internal sealed record RemoveUserFromPermissionCommand(RemoveUserFromPermissionRequest Request)
    : IRequest<Result>;

internal sealed class RemoveUserFromPermissionCommandHandler(
    AppManager appManager)
    : IRequestHandler<RemoveUserFromPermissionCommand, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(RemoveUserFromPermissionCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByIdAsync(request.Request.UserId);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        var permission = await appManager.PermissionManager.FindByNameAsync(request.Request.PermissionName, cancellationToken);

        if (permission is null)
        {
            return Results.NotFound($"Cannot find permission {request.Request.PermissionName}.");
        }

        var hasUserPermission =
            await appManager.PermissionManager.HasPermissionAsync(user, permission.Name, cancellationToken);

        if (!hasUserPermission)
        {
            return Result.Success("User does not have the permission.");
        }
        else
        {
            var permissionResult =
                await appManager.PermissionManager.RemoveFromPermissionAsync(user, permission, cancellationToken);

            if (!permissionResult.Succeeded)
            {
                return Results.InternalServerError($"Cannot remove user from permission {permission.Name} " +
                                                   $"due to server error: {permissionResult.Errors.First().Description}.");
            }

            return Result.Success("Successfully removed user form permission.");
        }
    }
}