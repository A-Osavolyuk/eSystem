namespace eShop.Domain.Requests.API.Auth;

public record GrantPermissionRequest
{
    public Guid UserId { get; set; }
    public HashSet<string> Permissions { get; set; } = [];
}