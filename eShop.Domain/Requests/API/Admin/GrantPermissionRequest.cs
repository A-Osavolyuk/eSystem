namespace eShop.Domain.Requests.API.Admin;

public record GrantPermissionRequest
{
    public Guid UserId { get; set; }
    public HashSet<string> Permissions { get; set; } = [];
}