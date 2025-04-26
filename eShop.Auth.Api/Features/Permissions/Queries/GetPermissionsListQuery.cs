namespace eShop.Auth.Api.Features.Permissions.Queries;

internal sealed record GetPermissionsListQuery() : IRequest<Result>;

internal sealed class GetPermissionsListQueryHandler(
    IPermissionManager permissionManager) : IRequestHandler<GetPermissionsListQuery, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;

    public async Task<Result> Handle(GetPermissionsListQuery request,
        CancellationToken cancellationToken)
    {
        var permissions = await permissionManager.GetListAsync(cancellationToken);

        return Result.Success(permissions.ToList());
    }
}