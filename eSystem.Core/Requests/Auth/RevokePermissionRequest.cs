namespace eSystem.Core.Requests.Auth;

public record RevokePermissionRequest
{
    public Guid UserId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
}