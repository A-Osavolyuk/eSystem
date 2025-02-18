namespace eShop.Auth.Api.Features.Admin.Commands;

internal sealed record DeleteUserAccountCommand(DeleteUserAccountRequest Request)
    : IRequest<Result<DeleteUserAccountResponse>>;

internal sealed class DeleteUserAccountCommandHandler(
    AppManager appManager) : IRequestHandler<DeleteUserAccountCommand, Result<DeleteUserAccountResponse>>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result<DeleteUserAccountResponse>> Handle(DeleteUserAccountCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByIdAsync(request.Request.UserId);

        if (user is null)
        {
            return new(new NotFoundException($"Cannot find user with ID {request.Request.UserId}"));
        }

        var rolesResult = await appManager.UserManager.RemoveFromRolesAsync(user);

        if (!rolesResult.Succeeded)
        {
            return new(new FailedOperationException(
                $"Cannot remove roles from user with ID {request.Request.UserId} " +
                $"due to server error: {rolesResult.Errors.First().Description}"));
        }

        var permissionsResult = await appManager.PermissionManager.RemoveFromPermissionsAsync(user);

        if (!permissionsResult.Succeeded)
        {
            return new(new FailedOperationException(
                $"Cannot remove permissions from user with ID {request.Request.UserId} " +
                $"due to server error: {permissionsResult.Errors.First().Description}"));
        }

        var personalDataResult = await appManager.ProfileManager.RemovePersonalDataAsync(user);

        if (!personalDataResult.Succeeded)
        {
            return new(new FailedOperationException(
                $"Failed on deleting the user account with message: {personalDataResult.Errors.First().Description}"));
        }

        var tokenResult = await appManager.SecurityManager.RemoveTokenAsync(user);

        if (!tokenResult.Succeeded)
        {
            return new(new FailedOperationException(
                $"Failed on deleting the user account with message: {tokenResult.Errors.First().Description}"));
        }

        var accountResult = await appManager.UserManager.DeleteAsync(user);

        if (!accountResult.Succeeded)
        {
            return new(new FailedOperationException(
                $"Cannot delete user account with ID {request.Request.UserId} " +
                $"due to server error: {accountResult.Errors.First().Description}"));
        }

        return new(
            new DeleteUserAccountResponse()
            {
                Message = "User account was successfully deleted.", Succeeded = true
            });
    }
}