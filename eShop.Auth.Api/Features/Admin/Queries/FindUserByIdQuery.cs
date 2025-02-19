namespace eShop.Auth.Api.Features.Admin.Queries;

internal sealed record FindUserByIdQuery(Guid UserId) : IRequest<Result<FindUserResponse>>;

internal sealed class FindUserByIdQueryHandler(
    AppManager appManager) : IRequestHandler<FindUserByIdQuery, Result<FindUserResponse>>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result<FindUserResponse>> Handle(FindUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByIdAsync(request.UserId);

        if (user is null)
        {
            return new(new NotFoundException($"Cannot find user with ID {request.UserId}."));
        }

        var accountData = Mapper.ToAccountData(user);
        var personalData = await appManager.ProfileManager.FindPersonalDataAsync(user);
        var rolesList = await appManager.UserManager.GetRolesAsync(user);
        var permissions = await appManager.PermissionManager.GetUserPermissionsAsync(user);

        if (!rolesList.Any())
        {
            return new(new NotFoundException($"Cannot find roles for user with ID {user.Id}."));
        }

        var permissionData = new PermissionsData() { Id = user.Id };

        foreach (var role in rolesList)
        {
            var roleInfo = await appManager.RoleManager.FindByNameAsync(role);

            if (roleInfo is null)
            {
                return new(new NotFoundException($"Cannot find role {role}"));
            }

            permissionData.Roles.Add(new RoleData()
            {
                Id = roleInfo.Id,
                Name = roleInfo.Name!,
                NormalizedName = roleInfo.NormalizedName!
            });
        }

        foreach (var permission in permissions)
        {
            var permissionInfo = await appManager.PermissionManager.FindPermissionAsync(permission);
            if (permissionInfo is null)
            {
                return new(new NotFoundException($"Cannot find permission {permission}."));
            }

            permissionData.Permissions.Add(new Permission()
            {
                Id = permissionInfo.Id,
                Name = permissionInfo.Name,
            });
        }

        var response = new FindUserResponse()
        {
            AccountData = accountData,
            PersonalDataEntity = personalData ?? new(),
            PermissionsData = permissionData
        };

        return new(response);
    }
}