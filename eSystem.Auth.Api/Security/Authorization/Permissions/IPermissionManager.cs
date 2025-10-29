using eSystem.Auth.Api.Data.Entities;

namespace eSystem.Auth.Api.Security.Authorization.Permissions;

public interface IPermissionManager
{
    public ValueTask<List<PermissionEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<PermissionEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask<PermissionEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<Result> GrantAsync(UserEntity user, PermissionEntity permission, CancellationToken cancellationToken = default);
    public ValueTask<Result> RevokeAsync(UserEntity user, PermissionEntity permissionEntity, CancellationToken cancellationToken = default);
}