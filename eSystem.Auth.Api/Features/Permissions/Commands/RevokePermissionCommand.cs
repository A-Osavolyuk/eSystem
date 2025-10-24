using eSystem.Domain.Requests.Auth;

namespace eSystem.Auth.Api.Features.Permissions.Commands;

public sealed record RevokePermissionCommand(RevokePermissionRequest Request)
    : IRequest<Result>;

public sealed class RemoveUserFromPermissionCommandHandler(
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
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var permission = await permissionManager.FindByNameAsync(request.Request.PermissionName, cancellationToken);
        if (permission is null) return Results.NotFound($"Cannot find permission {request.Request.PermissionName}.");
        
        var result = await permissionManager.RevokeAsync(user, permission, cancellationToken);
        return result;
    }
}