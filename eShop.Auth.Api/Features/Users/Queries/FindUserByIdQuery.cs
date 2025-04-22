using eShop.Domain.Common.API;
using eShop.Domain.Responses.API.Admin;

namespace eShop.Auth.Api.Features.Users.Queries;

internal sealed record FindUserByIdQuery(Guid UserId) : IRequest<Result>;

internal sealed class FindUserByIdQueryHandler(
    AppManager appManager) : IRequestHandler<FindUserByIdQuery, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(FindUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByIdAsync(request.UserId);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        }

        var accountData = Mapper.Map(user);
        var personalData = await appManager.ProfileManager.FindAsync(user, cancellationToken);
        var rolesList = await appManager.UserManager.GetRolesAsync(user);
        var permissions = await appManager.PermissionManager.GetUserPermissionsAsync(user, cancellationToken);

        if (!rolesList.Any())
        {
            return Results.NotFound($"Cannot find roles for user with ID {user.Id}.");
        }

        var permissionData = new PermissionsData() { Id = user.Id };

        foreach (var role in rolesList)
        {
            var roleInfo = await appManager.RoleManager.FindByNameAsync(role);

            if (roleInfo is null)
            {
                return Results.NotFound($"Cannot find role {role}");
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

        var response = new FindUserResponse()
        {
            AccountData = accountData,
            PersonalDataEntity = personalData ?? new(),
            PermissionsData = permissionData
        };

        return Result.Success(response);
    }
}