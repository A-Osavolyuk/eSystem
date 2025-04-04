namespace eShop.Domain.Requests.Api.Admin;

public record class IssuePermissionRequest
{
    public Guid UserId { get; set; }
    public HashSet<string> Permissions { get; set; } = new HashSet<string>();
}