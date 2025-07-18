using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Users.Queries;

public sealed record GetUserRolesQuery(Guid Id) : IRequest<Result>;

public sealed class GetUserRolesQueryHandler(
    IUserManager userManager,
    IRoleManager roleManager) : IRequestHandler<GetUserRolesQuery, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IRoleManager roleManager = roleManager;

    public async Task<Result> Handle(GetUserRolesQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Id}.");
        }

        var roles = await roleManager.GetByUserAsync(user, cancellationToken);

        if (!roles.Any())
        {
            return Results.NotFound($"Cannot find roles for user with ID {request.Id}.");
        }

        var result = roles.Select(Mapper.Map).ToList();

        return Result.Success(result);
    }
}