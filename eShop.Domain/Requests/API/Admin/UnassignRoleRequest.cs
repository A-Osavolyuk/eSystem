namespace eShop.Domain.Requests.API.Admin;

public record UnassignRoleRequest
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = null!;
}