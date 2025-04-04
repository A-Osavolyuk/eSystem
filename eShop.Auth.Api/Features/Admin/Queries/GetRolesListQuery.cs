namespace eShop.Auth.Api.Features.Admin.Queries;

internal sealed record GetRolesListQuery() : IRequest<Result>;

internal sealed class GetRolesListQueryHandler(
    AppManager appManager) : IRequestHandler<GetRolesListQuery, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(GetRolesListQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await appManager.RoleManager.Roles.ToListAsync(cancellationToken);
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

        return Result.Success(response);
    }
}