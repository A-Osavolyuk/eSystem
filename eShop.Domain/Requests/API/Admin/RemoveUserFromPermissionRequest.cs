namespace eShop.Domain.Requests.API.Admin;

public record RemoveUserFromPermissionRequest
{
    public Guid UserId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
}