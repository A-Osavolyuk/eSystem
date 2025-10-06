namespace eShop.Domain.Requests.Auth;

public record UnassignRolesRequest
{
    public Guid UserId { get; set; }
    public List<string> Roles { get; set; } = [];
}