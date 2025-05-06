using eShop.Domain.Responses.API.Admin;

namespace eShop.Auth.Api.Features.Users.Queries;

internal sealed record FindUserByIdQuery(Guid UserId) : IRequest<Result>;

internal sealed class FindUserByIdQueryHandler(
    IPermissionManager permissionManager,
    IProfileManager profileManager,
    UserManager<UserEntity> userManager,
    IRoleManager roleManager) : IRequestHandler<FindUserByIdQuery, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IProfileManager profileManager = profileManager;
    private readonly UserManager<UserEntity> userManager = userManager;
    private readonly IRoleManager roleManager = roleManager;


    public async Task<Result> Handle(FindUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        }

        var accountData = Mapper.Map(user);
        var personalData = await profileManager.FindAsync(user, cancellationToken);
        var roles = await roleManager.GetByUserAsync(user, cancellationToken);
        var permissions = await permissionManager.GetUserPermissionsAsync(user, cancellationToken);

        if (!roles.Any())
        {
            return Results.NotFound($"Cannot find roles for user with ID {user.Id}.");
        }

        var permissionData = new PermissionsData() { Id = user.Id };

        permissionData.Roles.AddRange(roles.Select(Mapper.Map).ToList());

        foreach (var permission in permissions)
        {
            var permissionInfo = await permissionManager.FindByNameAsync(permission, cancellationToken);
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