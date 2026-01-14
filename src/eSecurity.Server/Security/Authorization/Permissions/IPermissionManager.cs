using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.Permissions;

public interface IPermissionManager
{
    public ValueTask<List<UserPermissionsEntity>> GetAllAsync(UserEntity user,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> GrantAsync(UserEntity user, PermissionEntity permission,
        CancellationToken cancellationToken = default);
}