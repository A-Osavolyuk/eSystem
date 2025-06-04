namespace eShop.Auth.Api.Features.Users.Commands;

internal sealed record DeleteUserAccountCommand(Guid Id)
    : IRequest<Result>;

internal sealed class DeleteUserAccountCommandHandler(
    IPermissionManager permissionManager,
    IPersonalDataManager personalDataManager,
    ITokenManager tokenManager,
    IRoleManager roleManager,
    IUserManager userManager) : IRequestHandler<DeleteUserAccountCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IPersonalDataManager personalDataManager = personalDataManager;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IRoleManager roleManager = roleManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(DeleteUserAccountCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Id}");
        }

        var rolesResult = await roleManager.UnassignRolesAsync(user, cancellationToken);

        if (!rolesResult.Succeeded)
        {
            return rolesResult;
        }

        var permissionsResult = await permissionManager.RevokeAsync(user, cancellationToken);

        if (!permissionsResult.Succeeded)
        {
            return permissionsResult;
        }
        
        var personalDataEntity = await personalDataManager.FindAsync(user, cancellationToken);

        if (personalDataEntity is null)
        {
            return Results.NotFound("Not found personal data");
        }

        var personalDataResult = await personalDataManager.DeleteAsync(personalDataEntity, cancellationToken);

        if (!personalDataResult.Succeeded)
        {
            return personalDataResult;
        }

        var tokenResult = await tokenManager.RemoveAsync(user, cancellationToken);

        if (!tokenResult.Succeeded)
        {
            return tokenResult;
        }

        var accountResult = await userManager.DeleteAsync(user, cancellationToken);

        return accountResult;
    }
}