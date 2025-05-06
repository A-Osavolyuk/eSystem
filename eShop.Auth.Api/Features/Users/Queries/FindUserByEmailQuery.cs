using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Features.Users.Queries;

internal sealed record FindUserByEmailQuery(string Email) : IRequest<Result>;

internal sealed class FindUserByEmailQueryHandler(
    UserManager<UserEntity> userManager,
    IRoleManager roleManager,
    IProfileManager profileManager,
    IPermissionManager permissionManager) : IRequestHandler<FindUserByEmailQuery, Result>
{
    private readonly UserManager<UserEntity> userManager = userManager;
    private readonly IRoleManager roleManager = roleManager;
    private readonly IProfileManager profileManager = profileManager;
    private readonly IPermissionManager permissionManager = permissionManager;

    public async Task<Result> Handle(FindUserByEmailQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Email}.");
        }

        var accountData = Mapper.Map(user);
        var personalData = await profileManager.FindAsync(user, cancellationToken);
        var roles = await roleManager.GetByUserAsync(user, cancellationToken);
        var permissions = await permissionManager.GetByUserAsync(user, cancellationToken);

        if (!roles.Any())
        {
            return Results.NotFound($"Cannot find roles for user with email {user.Email}.");
        }

        var permissionData = new PermissionsData() { Id = user.Id };

        permissionData.Roles.AddRange(roles.Select(Mapper.Map).ToList());
        permissionData.Permissions.AddRange(permissions.Select(Mapper.Map).ToList());

        var response = new UserDto()
        {
            AccountData = accountData,
            PersonalData = personalData ?? new PersonalData(),
            PermissionsData = permissionData
        };

        return Result.Success(response);
    }
}