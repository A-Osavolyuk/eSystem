namespace eShop.Auth.Api.Interfaces;

public interface IPermissionManager
{
    public ValueTask<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);

    public ValueTask<bool> HasAsync(UserEntity userEntity, string name, CancellationToken cancellationToken = default);
    public ValueTask<List<PermissionEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public ValueTask<List<PermissionEntity>> GetByUserAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<PermissionEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask<PermissionEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<Result> IssueAsync(UserEntity user, List<PermissionEntity> collection, CancellationToken cancellationToken = default);
    public ValueTask<Result> IssueAsync(UserEntity user, PermissionEntity permission, CancellationToken cancellationToken = default);
    public ValueTask<Result> RemoveAsync(UserEntity userEntity, PermissionEntity permissionEntity, CancellationToken cancellationToken = default);
    public ValueTask<Result> RemoveAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
}