namespace eSystem.Auth.Api.Features.Roles.Queries;

public sealed record GetRolesQuery() : IRequest<Result>;

public sealed class GetRolesListQueryHandler(
    IRoleManager roleManager) : IRequestHandler<GetRolesQuery, Result>
{
    private readonly IRoleManager roleManager = roleManager;

    public async Task<Result> Handle(GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await roleManager.GetAllAsync(cancellationToken);
        var result = roles.Select(Mapper.Map).ToList();

        return Result.Success(result);
    }
}