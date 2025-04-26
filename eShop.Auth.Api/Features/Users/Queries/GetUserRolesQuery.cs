using eShop.Domain.Responses.API.Admin;

namespace eShop.Auth.Api.Features.Users.Queries;

internal sealed record GetUserRolesQuery(Guid Id) : IRequest<Result>;

internal sealed class GetUserRolesQueryHandler(
    UserManager<UserEntity> userManager,
    RoleManager<RoleEntity> roleManager) : IRequestHandler<GetUserRolesQuery, Result>
{
    private readonly UserManager<UserEntity> userManager = userManager;
    private readonly RoleManager<RoleEntity> roleManager = roleManager;

    public async Task<Result> Handle(GetUserRolesQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Id}.");
        }

        var roleList = await userManager.GetRolesAsync(user);

        if (!roleList.Any())
        {
            return Results.NotFound($"Cannot find roles for user with ID {request.Id}.");
        }

        var result = new UserRolesResponse { UserId = user.Id };

        foreach (var role in roleList)
        {
            var roleInfo = await roleManager.FindByNameAsync(role);

            if (roleInfo is null)
            {
                return Results.NotFound($"Cannot find role {role}");
            }

            result.Roles.Add(new RoleData()
            {
                Id = roleInfo.Id,
                Name = roleInfo.Name!,
                NormalizedName = roleInfo.NormalizedName!
            });
        }

        return Result.Success(result);
    }
}