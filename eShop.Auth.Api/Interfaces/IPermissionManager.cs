namespace eShop.Auth.Api.Interfaces;

internal interface IPermissionManager
{
    public ValueTask<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask<bool> HasPermissionAsync(UserEntity userEntity, string name, CancellationToken cancellationToken = default);
    public ValueTask<List<PermissionEntity>> GetListAsync(CancellationToken cancellationToken = default);
    public ValueTask<List<string>> GetUserPermissionsAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
    public ValueTask<PermissionEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask<IdentityResult> IssueAsync(UserEntity userEntity, IEnumerable<string> collection, CancellationToken cancellationToken = default);
    public ValueTask<IdentityResult> IssueAsync(UserEntity userEntity, string permission, CancellationToken cancellationToken = default);
    public ValueTask<IdentityResult> RemoveFromPermissionAsync(UserEntity userEntity, PermissionEntity permissionEntity, CancellationToken cancellationToken = default);
    public ValueTask<IdentityResult> RemoveFromPermissionsAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
}