namespace eShop.Domain.Requests.API.Auth;

public record UnassignRolesRequest
{
    public Guid UserId { get; set; }
    public List<string> Roles { get; set; } = [];
}