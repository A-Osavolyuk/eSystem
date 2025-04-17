using eShop.Domain.Common.API;

namespace eShop.Auth.Api.Features.Admin.Queries;

internal sealed record GetPermissionsListQuery() : IRequest<Result>;

internal sealed class GetPermissionsListQueryHandler(
    AppManager appManager) : IRequestHandler<GetPermissionsListQuery, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(GetPermissionsListQuery request,
        CancellationToken cancellationToken)
    {
        var permissions = await appManager.PermissionManager.GetListAsync(cancellationToken);

        return Result.Success(permissions.ToList());
    }
}