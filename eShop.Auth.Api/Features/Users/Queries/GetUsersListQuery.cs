using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Features.Users.Queries;

internal sealed record GetUsersListQuery() : IRequest<Result>;

internal sealed class GetUsersListQueryHandler(
    IPermissionManager permissionManager,
    IProfileManager profileManager,
    UserManager<UserEntity> userManager,
    IRoleManager roleManager) : IRequestHandler<GetUsersListQuery, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IProfileManager profileManager = profileManager;
    private readonly UserManager<UserEntity> userManager = userManager;
    private readonly IRoleManager roleManager = roleManager;

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
            var roles = await roleManager.GetByUserAsync(user, cancellationToken);
            var permissions = await permissionManager.GetByUserAsync(user, cancellationToken);
            
            var permissionData = new PermissionsData()
            {
                Roles = roles.Select(Mapper.Map).ToList(),
                Permissions = permissions.Select(Mapper.Map).ToList()
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