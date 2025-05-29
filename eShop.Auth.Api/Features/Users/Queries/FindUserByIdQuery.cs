using eShop.Domain.DTOs;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Users.Queries;

internal sealed record FindUserByIdQuery(Guid UserId) : IRequest<Result>;

internal sealed class FindUserByIdQueryHandler(
    IPermissionManager permissionManager,
    IProfileManager profileManager,
    IUserManager userManager,
    IRoleManager roleManager) : IRequestHandler<FindUserByIdQuery, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IProfileManager profileManager = profileManager;
    private readonly IUserManager userManager = userManager;
    private readonly IRoleManager roleManager = roleManager;


    public async Task<Result> Handle(FindUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        }

        var accountData = Mapper.Map(user);
        var personalData = await profileManager.FindAsync(user, cancellationToken);
        var roles = await roleManager.GetByUserAsync(user, cancellationToken);
        var permissions = await permissionManager.GetByUserAsync(user, cancellationToken);

        if (!roles.Any())
        {
            return Results.NotFound($"Cannot find roles for user with ID {user.Id}.");
        }

        var permissionData = new PermissionsDataDto() { Id = user.Id };

        permissionData.Roles.AddRange(roles.Select(Mapper.Map).ToList());
        permissionData.Permissions.AddRange(permissions.Select(Mapper.Map).ToList());

        //TODO: Load user data
        
        var response = new FindUserResponse()
        {
            AccountDataDto = accountData,
            PersonalDataDtoEntity = new(),
            PermissionsDataDto = permissionData
        };

        return Result.Success(response);
    }
}