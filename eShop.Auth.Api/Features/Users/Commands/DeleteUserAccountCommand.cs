using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Users.Commands;

internal sealed record DeleteUserAccountCommand(DeleteUserAccountRequest Request)
    : IRequest<Result>;

internal sealed class DeleteUserAccountCommandHandler(
    IPermissionManager permissionManager,
    IProfileManager profileManager,
    ITokenManager tokenManager,
    UserManager<UserEntity> userManager) : IRequestHandler<DeleteUserAccountCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IProfileManager profileManager = profileManager;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly UserManager<UserEntity> userManager = userManager;

    public async Task<Result> Handle(DeleteUserAccountCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        }

        var rolesResult = await userManager.RemoveFromRolesAsync(user);

        if (!rolesResult.Succeeded)
        {
            return Results.InternalServerError($"Cannot remove roles from user with ID {request.Request.UserId} " +
                                               $"due to server error: {rolesResult.Errors.First().Description}");
        }

        var permissionsResult = await permissionManager.RemoveFromPermissionsAsync(user, cancellationToken);

        if (!permissionsResult.Succeeded)
        {
            return Results.InternalServerError(
                $"Cannot remove permissions from user with ID {request.Request.UserId} " +
                $"due to server error: {permissionsResult.Errors.First().Description}");
        }

        var personalDataResult = await profileManager.DeleteAsync(user, cancellationToken);

        if (!personalDataResult.Succeeded)
        {
            return Results.InternalServerError(
                $"Failed on deleting the user account with message: {personalDataResult.Errors.First().Description}");
        }

        var tokenResult = await tokenManager.RemoveAsync(user, cancellationToken);

        if (!tokenResult.Succeeded)
        {
            return tokenResult;
        }

        var accountResult = await userManager.DeleteAsync(user);

        if (!accountResult.Succeeded)
        {
            return Results.InternalServerError($"Cannot delete user account with ID {request.Request.UserId} " +
                                               $"due to server error: {accountResult.Errors.First().Description}");
        }

        return Result.Success("User account was successfully deleted.");
    }
}