namespace eShop.Domain.Requests.Api.Admin;

public record class RemoveUserFromPermissionRequest
{
    public Guid UserId { get; set; }
    public string PermissionName { get; set; } = string.Empty;
}