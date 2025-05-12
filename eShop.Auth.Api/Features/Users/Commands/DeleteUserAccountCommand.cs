using eShop.Domain.Requests.API.Admin;

namespace eShop.Auth.Api.Features.Users.Commands;

internal sealed record DeleteUserAccountCommand(DeleteUserAccountRequest Request)
    : IRequest<Result>;

internal sealed class DeleteUserAccountCommandHandler(
    IPermissionManager permissionManager,
    IProfileManager profileManager,
    ISecurityTokenManager securityTokenManager,
    IUserManager userManager) : IRequestHandler<DeleteUserAccountCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IProfileManager profileManager = profileManager;
    private readonly ISecurityTokenManager securityTokenManager = securityTokenManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(DeleteUserAccountCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        }

        var rolesResult = await userManager.RemoveFromRolesAsync(user, cancellationToken);

        if (!rolesResult.Succeeded)
        {
            return rolesResult;
        }

        var permissionsResult = await permissionManager.RemoveAsync(user, cancellationToken);

        if (!permissionsResult.Succeeded)
        {
            return permissionsResult;
        }

        var personalDataResult = await profileManager.DeleteAsync(user, cancellationToken);

        if (!personalDataResult.Succeeded)
        {
            return Results.InternalServerError(
                $"Failed on deleting the user account with message: {personalDataResult.Errors.First().Description}");
        }

        var tokenResult = await securityTokenManager.RemoveAsync(user, cancellationToken);

        if (!tokenResult.Succeeded)
        {
            return tokenResult;
        }

        var accountResult = await userManager.DeleteAsync(user, cancellationToken);

        if (!accountResult.Succeeded)
        {
            return accountResult;
        }

        return Result.Success("User account was successfully deleted.");
    }
}