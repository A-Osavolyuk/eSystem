using eShop.Domain.Common.Results;

namespace eShop.Auth.Api.Features.Permissions.Queries;

public sealed record GetPermissionsQuery() : IRequest<Result>;

public sealed class GetPermissionsQueryHandler(
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