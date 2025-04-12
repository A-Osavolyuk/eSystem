namespace eShop.Domain.Requests.API.Admin;

public record class IssuePermissionRequest
{
    public Guid UserId { get; set; }
    public HashSet<string> Permissions { get; set; } = new();
}