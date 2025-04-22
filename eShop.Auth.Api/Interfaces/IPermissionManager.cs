namespace eShop.Auth.Api.Interfaces;

internal interface IPermissionManager
{
    public ValueTask<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask<bool> HasPermissionAsync(AppUser user, string name, CancellationToken cancellationToken = default);
    public ValueTask<List<PermissionEntity>> GetListAsync(CancellationToken cancellationToken = default);
    public ValueTask<List<string>> GetUserPermissionsAsync(AppUser user, CancellationToken cancellationToken = default);
    public ValueTask<PermissionEntity?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask<IdentityResult> IssueAsync(AppUser user, IEnumerable<string> collection, CancellationToken cancellationToken = default);
    public ValueTask<IdentityResult> IssueAsync(AppUser user, string permission, CancellationToken cancellationToken = default);
    public ValueTask<IdentityResult> RemoveFromPermissionAsync(AppUser user, PermissionEntity permissionEntity, CancellationToken cancellationToken = default);
    public ValueTask<IdentityResult> RemoveFromPermissionsAsync(AppUser user, CancellationToken cancellationToken = default);
}