namespace eShop.Auth.Api.Features.Permissions.Queries;

internal sealed record GetPermissionsQuery() : IRequest<Result>;

internal sealed class GetPermissionsQueryHandler(
    IPermissionManager permissionManager) : IRequestHandler<GetPermissionsQuery, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;

    public async Task<Result> Handle(GetPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        var permissions = await permissionManager.GetAllAsync(cancellationToken);

        return Result.Success(permissions.ToList());
    }
}