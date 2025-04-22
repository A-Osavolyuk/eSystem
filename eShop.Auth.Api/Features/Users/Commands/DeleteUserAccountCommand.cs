using eShop.Domain.Common.API;
using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Users.Commands;

internal sealed record DeleteUserAccountCommand(DeleteUserAccountRequest Request)
    : IRequest<Result>;

internal sealed class DeleteUserAccountCommandHandler(
    AppManager appManager) : IRequestHandler<DeleteUserAccountCommand, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(DeleteUserAccountCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByIdAsync(request.Request.UserId);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        }

        var rolesResult = await appManager.UserManager.RemoveFromRolesAsync(user);

        if (!rolesResult.Succeeded)
        {
            return Results.InternalServerError($"Cannot remove roles from user with ID {request.Request.UserId} " +
                                               $"due to server error: {rolesResult.Errors.First().Description}");
        }

        var permissionsResult = await appManager.PermissionManager.RemoveFromPermissionsAsync(user, cancellationToken);

        if (!permissionsResult.Succeeded)
        {
            return Results.InternalServerError(
                $"Cannot remove permissions from user with ID {request.Request.UserId} " +
                $"due to server error: {permissionsResult.Errors.First().Description}");
        }

        var personalDataResult = await appManager.ProfileManager.DeleteAsync(user, cancellationToken);

        if (!personalDataResult.Succeeded)
        {
            return Results.InternalServerError(
                $"Failed on deleting the user account with message: {personalDataResult.Errors.First().Description}");
        }

        var tokenResult = await appManager.SecurityManager.RemoveTokenAsync(user);

        if (!tokenResult.Succeeded)
        {
            return Results.InternalServerError(
                $"Failed on deleting the user account with message: {tokenResult.Errors.First().Description}");
        }

        var accountResult = await appManager.UserManager.DeleteAsync(user);

        if (!accountResult.Succeeded)
        {
            return Results.InternalServerError($"Cannot delete user account with ID {request.Request.UserId} " +
                                               $"due to server error: {accountResult.Errors.First().Description}");
        }

        return Result.Success("User account was successfully deleted.");
    }
}