namespace eShop.Auth.Api.Features.Admin.Queries;

internal sealed record GetUsersListQuery() : IRequest<Result<IEnumerable<UserDto>>>;

internal sealed class GetUsersListQueryHandler(
    AppManager appManager) : IRequestHandler<GetUsersListQuery, Result<IEnumerable<UserDto>>>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result<IEnumerable<UserDto>>> Handle(GetUsersListQuery request,
        CancellationToken cancellationToken)
    {
        var usersList = await appManager.UserManager.Users.AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);

        if (!usersList.Any())
        {
            return new(new List<UserDto>());
        }

        var users = new List<UserDto>();

        foreach (var user in usersList)
        {
            var accountData = Mapper.ToAccountData(user);
            var personalData = await appManager.ProfileManager.FindPersonalDataAsync(user);
            var rolesList = (await appManager.UserManager.GetRolesAsync(user)).ToList();

            if (!rolesList.Any())
            {
                return new(new NotFoundException($"Cannot find roles for user with ID {user.Id}."));
            }

            var rolesData = (await appManager.RoleManager.GetRolesDataAsync(rolesList) ?? Array.Empty<RoleData>())
                .ToList();
            var permissions = await appManager.PermissionManager.GetUserPermissionsAsync(user);
            var roleInfos = rolesData.ToList();

            if (!roleInfos.Any())
            {
                return new(new NotFoundException("Cannot find roles data."));
            }

            var permissionsList = new List<Permission>();

            foreach (var permission in permissions)
            {
                var permissionInfo = await appManager.PermissionManager.FindPermissionAsync(permission);

                if (permissionInfo is null)
                {
                    return new(new NotFoundException($"Cannot find permission {permission}."));
                }

                permissionsList.Add(new Permission()
                {
                    Id = permissionInfo.Id,
                    Name = permissionInfo.Name,
                });
            }

            var permissionData = new PermissionsData()
            {
                Roles = roleInfos.ToList(),
                Permissions = permissionsList
            };

            users.Add(new UserDto()
            {
                PermissionsData = permissionData,
                PersonalData = personalData ?? new(),
                AccountData = accountData
            });
        }

        return users;
    }
}