namespace eShop.Auth.Api.Features.Admin.Queries;

internal sealed record GetRolesListQuery() : IRequest<Result<IEnumerable<RoleDto>>>;

internal sealed class GetRolesListQueryHandler(
    AppManager appManager) : IRequestHandler<GetRolesListQuery, Result<IEnumerable<RoleDto>>>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result<IEnumerable<RoleDto>>> Handle(GetRolesListQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await appManager.RoleManager.Roles.ToListAsync(cancellationToken);

        if (roles.Count == 0)
        {
            return new(new NotFoundException("Cannot find roles."));
        }

        var response = new List<RoleDto>();
        foreach (var role in roles)
        {
            var memberCount = await appManager.RoleManager.Roles
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

        return new(response);
    }
}