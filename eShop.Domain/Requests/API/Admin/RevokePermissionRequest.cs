namespace eShop.Domain.Requests.API.Admin;

public record RevokePermissionRequest
{
    public Guid UserId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
}