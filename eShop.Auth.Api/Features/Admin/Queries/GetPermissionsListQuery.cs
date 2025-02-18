namespace eShop.Auth.Api.Features.Admin.Queries;

internal sealed record GetPermissionsListQuery() : IRequest<Result<IEnumerable<PermissionEntity>>>;

internal sealed class GetPermissionsListQueryHandler(
    AppManager appManager) : IRequestHandler<GetPermissionsListQuery, Result<IEnumerable<PermissionEntity>>>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result<IEnumerable<PermissionEntity>>> Handle(GetPermissionsListQuery request,
        CancellationToken cancellationToken)
    {
        var permissions = await appManager.PermissionManager.GetPermissionsAsync();

        if (!permissions.Any())
        {
            return new(new NotFoundException("Cannot find permissions."));
        }

        return permissions.ToList();
    }
}