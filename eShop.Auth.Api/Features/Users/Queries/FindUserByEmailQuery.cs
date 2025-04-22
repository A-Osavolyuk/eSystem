using eShop.Domain.Common.API;
using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Features.Users.Queries;

internal sealed record FindUserByEmailQuery(string Email) : IRequest<Result>;

internal sealed class FindUserByEmailQueryHandler(
    AppManager appManager) : IRequestHandler<FindUserByEmailQuery, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(FindUserByEmailQuery request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Email}.");
        }

        var accountData = Mapper.Map(user);
        var personalData = await appManager.ProfileManager.FindAsync(user, cancellationToken);
        var rolesList = await appManager.UserManager.GetRolesAsync(user);
        var permissions = await appManager.PermissionManager.GetUserPermissionsAsync(user, cancellationToken);

        if (!rolesList.Any())
        {
            return Results.NotFound($"Cannot find roles for user with email {user.Email}.");
        }

        var permissionData = new PermissionsData() { Id = user.Id };
        foreach (var role in rolesList)
        {
            var roleInfo = await appManager.RoleManager.FindByNameAsync(role);

            if (roleInfo is null)
            {
                return Results.NotFound($"Cannot find role {role}.");
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
            var permissionInfo = await appManager.PermissionManager.FindByNameAsync(permission, cancellationToken);

            if (permissionInfo is null)
            {
                return Results.NotFound($"Cannot find permission {permission}.");
            }

            permissionData.Permissions.Add(new Permission()
            {
                Id = permissionInfo.Id,
                Name = permissionInfo.Name,
            });
        }

        var response = new UserDto()
        {
            AccountData = accountData,
            PersonalData = personalData ?? new PersonalData(),
            PermissionsData = permissionData
        };

        return Result.Success(response);
    }
}