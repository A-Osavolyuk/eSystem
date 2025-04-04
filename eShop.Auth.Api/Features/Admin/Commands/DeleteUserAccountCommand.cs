namespace eShop.Auth.Api.Features.Admin.Commands;

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
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with ID {request.Request.UserId}"
            });
        }

        var rolesResult = await appManager.UserManager.RemoveFromRolesAsync(user);

        if (!rolesResult.Succeeded)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.InternalServerError,
                Message = "Server error",
                Details = $"Cannot remove roles from user with ID {request.Request.UserId} " +
                          $"due to server error: {rolesResult.Errors.First().Description}"
            });
        }

        var permissionsResult = await appManager.PermissionManager.RemoveFromPermissionsAsync(user);

        if (!permissionsResult.Succeeded)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.InternalServerError,
                Message = "Server error",
                Details = $"Cannot remove permissions from user with ID {request.Request.UserId} " +
                          $"due to server error: {permissionsResult.Errors.First().Description}"
            });
        }

        var personalDataResult = await appManager.ProfileManager.RemovePersonalDataAsync(user);

        if (!personalDataResult.Succeeded)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.InternalServerError,
                Message = "Server error",
                Details =
                    $"Failed on deleting the user account with message: {personalDataResult.Errors.First().Description}"
            });
        }

        var tokenResult = await appManager.SecurityManager.RemoveTokenAsync(user);

        if (!tokenResult.Succeeded)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.InternalServerError,
                Message = "Server error",
                Details = $"Failed on deleting the user account with message: {tokenResult.Errors.First().Description}"
            });
        }

        var accountResult = await appManager.UserManager.DeleteAsync(user);

        if (!accountResult.Succeeded)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.InternalServerError,
                Message = "Server error",
                Details = $"Cannot delete user account with ID {request.Request.UserId} " +
                          $"due to server error: {accountResult.Errors.First().Description}"
            });
        }

        return Result.Success("User account was successfully deleted.");
    }
}