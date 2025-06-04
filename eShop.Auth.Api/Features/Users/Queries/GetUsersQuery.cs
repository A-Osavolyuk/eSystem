using eShop.Domain.DTOs;
using UserDto = eShop.Domain.DTOs.UserDto;

namespace eShop.Auth.Api.Features.Users.Queries;

internal sealed record GetUsersQuery() : IRequest<Result>;

internal sealed class GetUsersQueryHandler(
    IPermissionManager permissionManager,
    IPersonalDataManager personalDataManager,
    IUserManager userManager,
    IRoleManager roleManager) : IRequestHandler<GetUsersQuery, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IPersonalDataManager personalDataManager = personalDataManager;
    private readonly IUserManager userManager = userManager;
    private readonly IRoleManager roleManager = roleManager;

    public async Task<Result> Handle(GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var usersList = await userManager.GetAllAsync(cancellationToken);

        if (!usersList.Any())
        {
            return Result.Success(new List<UserDto>());
        }

        var users = new List<UserDto>();

        foreach (var user in usersList)
        {
            var accountData = Mapper.Map(user);
            var personalData = await personalDataManager.FindAsync(user, cancellationToken);
            var roles = await roleManager.GetByUserAsync(user, cancellationToken);
            var permissions = await permissionManager.GetByUserAsync(user, cancellationToken);
            
            var permissionData = new PermissionsDataDto()
            {
                Roles = roles.Select(Mapper.Map).ToList(),
                Permissions = permissions.Select(Mapper.Map).ToList()
            };

            users.Add(new UserDto()
            {
                PermissionsDataDto = permissionData,
                PersonalDataDto = new(),
                AccountDataDto = accountData
            });
        }

        return Result.Success(users);
    }
}