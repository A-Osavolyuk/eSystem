namespace eShop.Domain.Requests.API.Admin;

public record RemoveUserRoleRequest
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = null!;
}