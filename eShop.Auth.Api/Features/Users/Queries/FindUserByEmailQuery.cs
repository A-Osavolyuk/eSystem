using eShop.Domain.DTOs;
using UserDto = eShop.Domain.DTOs.UserDto;

namespace eShop.Auth.Api.Features.Users.Queries;

internal sealed record FindUserByEmailQuery(string Email) : IRequest<Result>;

internal sealed class FindUserByEmailQueryHandler(
    IUserManager userManager,
    IRoleManager roleManager,
    IProfileManager profileManager,
    IPermissionManager permissionManager) : IRequestHandler<FindUserByEmailQuery, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IRoleManager roleManager = roleManager;
    private readonly IProfileManager profileManager = profileManager;
    private readonly IPermissionManager permissionManager = permissionManager;

    public async Task<Result> Handle(FindUserByEmailQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email, cancellationToken);

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

        var permissionData = new PermissionsDataDto() { Id = user.Id };

        permissionData.Roles.AddRange(roles.Select(Mapper.Map).ToList());
        permissionData.Permissions.AddRange(permissions.Select(Mapper.Map).ToList());

        //TODO: Load user data
        
        var response = new UserDto()
        {
            AccountDataDto = accountData,
            PersonalDataDto = new PersonalDataDto(),
            PermissionsDataDto = permissionData
        };

        return Result.Success(response);
    }
}