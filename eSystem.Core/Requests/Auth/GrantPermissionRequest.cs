namespace eSystem.Core.Requests.Auth;

public record GrantPermissionRequest
{
    public Guid UserId { get; set; }
    public HashSet<string> Permissions { get; set; } = [];
}