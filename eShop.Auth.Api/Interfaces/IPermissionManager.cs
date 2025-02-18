namespace eShop.Auth.Api.Interfaces;

internal interface IPermissionManager
{
    public ValueTask<bool> ExistsAsync(string name);
    public ValueTask<bool> HasPermissionAsync(AppUser user, string name);
    public ValueTask<List<PermissionEntity>> GetPermissionsAsync();
    public ValueTask<List<string>> GetUserPermissionsAsync(AppUser user);
    public ValueTask<PermissionEntity?> FindPermissionAsync(string name);
    public ValueTask<IdentityResult> IssuePermissionsAsync(AppUser user, IList<string> permissions);
    public ValueTask<IdentityResult> IssuePermissionAsync(AppUser user, string permission);
    public ValueTask<IdentityResult> RemoveFromPermissionAsync(AppUser user, PermissionEntity permissionEntity);
    public ValueTask<IdentityResult> RemoveFromPermissionsAsync(AppUser user);
}