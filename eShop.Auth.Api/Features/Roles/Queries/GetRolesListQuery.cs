using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Features.Roles.Queries;

internal sealed record GetRolesListQuery() : IRequest<Result>;

internal sealed class GetRolesListQueryHandler(
    IRoleManager roleManager) : IRequestHandler<GetRolesListQuery, Result>
{
    private readonly IRoleManager roleManager = roleManager;

    public async Task<Result> Handle(GetRolesListQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await roleManager.GetAllAsync(cancellationToken);
        var result = roles.Select(Mapper.Map).ToList();

        return Result.Success(result);
    }
}