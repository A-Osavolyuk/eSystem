using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Features.Users.Queries;

internal sealed record GetUsersListQuery() : IRequest<Result>;

internal sealed class GetUsersListQueryHandler(
    IPermissionManager permissionManager,
    IProfileManager profileManager,
    UserManager<UserEntity> userManager,
    RoleManager<RoleEntity> roleManager) : IRequestHandler<GetUsersListQuery, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IProfileManager profileManager = profileManager;
    private readonly UserManager<UserEntity> userManager = userManager;
    private readonly RoleManager<RoleEntity> roleManager = roleManager;

    public async Task<Result> Handle(GetUsersListQuery request,
        CancellationToken cancellationToken)
    {
        var usersList = await userManager.Users.AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);

        if (!usersList.Any())
        {
            return Result.Success(new List<UserDto>());
        }

        var users = new List<UserDto>();

        foreach (var user in usersList)
        {
            var accountData = Mapper.Map(user);
            var personalData = await profileManager.FindAsync(user, cancellationToken);
            var rolesList = (await userManager.GetRolesAsync(user)).ToList();

            if (!rolesList.Any())
            {
                return Results.NotFound($"Cannot find roles for user with ID {user.Id}.");
            }

            var rolesData = await roleManager.GetRolesDataAsync(rolesList) ?? [];
            var permissions = await permissionManager.GetUserPermissionsAsync(user, cancellationToken);

            if (!rolesData.Any())
            {
                return Results.NotFound("Cannot find roles data.");
            }

            var permissionsList = new List<Permission>();

            foreach (var permission in permissions)
            {
                var permissionInfo = await permissionManager.FindByNameAsync(permission, cancellationToken);

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
                Roles = rolesData,
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