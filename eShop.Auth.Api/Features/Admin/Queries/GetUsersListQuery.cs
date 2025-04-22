using eShop.Domain.Common.API;
using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Features.Admin.Queries;

internal sealed record GetUsersListQuery() : IRequest<Result>;

internal sealed class GetUsersListQueryHandler(
    AppManager appManager) : IRequestHandler<GetUsersListQuery, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(GetUsersListQuery request,
        CancellationToken cancellationToken)
    {
        var usersList = await appManager.UserManager.Users.AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);

        if (!usersList.Any())
        {
            return Result.Success(new List<UserDto>());
        }

        var users = new List<UserDto>();

        foreach (var user in usersList)
        {
            var accountData = Mapper.ToAccountData(user);
            var personalData = await appManager.ProfileManager.FindAsync(user, cancellationToken);
            var rolesList = (await appManager.UserManager.GetRolesAsync(user)).ToList();

            if (!rolesList.Any())
            {
                return Results.NotFound($"Cannot find roles for user with ID {user.Id}.");
            }

            var rolesData = await appManager.RoleManager.GetRolesDataAsync(rolesList) ?? [];
            var permissions = await appManager.PermissionManager.GetUserPermissionsAsync(user, cancellationToken);
            var roleInfos = rolesData.ToList();

            if (!roleInfos.Any())
            {
                return Results.NotFound("Cannot find roles data.");
            }

            var permissionsList = new List<Permission>();

            foreach (var permission in permissions)
            {
                var permissionInfo = await appManager.PermissionManager.FindByNameAsync(permission, cancellationToken);

                if (permissionInfo is null)
                {
                    return Results.NotFound($"Cannot find permission {permission}.");
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

        return Result.Success(users);
    }
}