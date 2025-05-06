using eShop.Domain.Responses.API.Admin;

namespace eShop.Auth.Api.Features.Users.Queries;

internal sealed record GetUserRolesQuery(Guid Id) : IRequest<Result>;

internal sealed class GetUserRolesQueryHandler(
    UserManager<UserEntity> userManager,
    IRoleManager roleManager) : IRequestHandler<GetUserRolesQuery, Result>
{
    private readonly UserManager<UserEntity> userManager = userManager;
    private readonly IRoleManager roleManager = roleManager;

    public async Task<Result> Handle(GetUserRolesQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Id}.");
        }

        var roles = await roleManager.GetByUserAsync(user, cancellationToken);

        if (!roles.Any())
        {
            return Results.NotFound($"Cannot find roles for user with ID {request.Id}.");
        }

        var result = new UserRolesResponse
        {
            UserId = user.Id, 
            Roles = roles.Select(Mapper.Map).ToList()
        };

        return Result.Success(result);
    }
}