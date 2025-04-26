using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Features.Roles.Queries;

internal sealed record GetRolesListQuery() : IRequest<Result>;

internal sealed class GetRolesListQueryHandler(
    RoleManager<RoleEntity> roleManager) : IRequestHandler<GetRolesListQuery, Result>
{
    private readonly RoleManager<RoleEntity> roleManager = roleManager;

    public async Task<Result> Handle(GetRolesListQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.ToListAsync(cancellationToken);
        var response = new List<RoleDto>();
        foreach (var role in roles)
        {
            var memberCount = await roleManager.Roles
                .Where(x => x.Id == role.Id)
                .CountAsync(cancellationToken: cancellationToken);

            response.Add(new()
            {
                Id = role!.Id,
                Name = role.Name!,
                NormalizedName = role.NormalizedName!,
                MembersCount = memberCount,
            });
        }

        return Result.Success(response);
    }
}